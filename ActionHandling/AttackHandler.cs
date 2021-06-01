﻿using System;
using System.Linq;
using ActionHandling.DTO;
using DatabaseHandler;
using DatabaseHandler.POCO;
using DatabaseHandler.Repository;
using DatabaseHandler.Services;
using Items;
using Network;
using Network.DTO;
using Newtonsoft.Json;
using WorldGeneration;

namespace ActionHandling
{
    public class AttackHandler : IAttackHandler, IPacketHandler
    {
        private IClientController _clientController;
        private string _playerGuid;
        private IWorldService _worldService;
        const int ATTACKSTAMINA = 10;

        public AttackHandler(IClientController clientController, IWorldService worldService)
        {
            _clientController = clientController;
            _clientController.SubscribeToPacketType(this, PacketType.Attack);
            _worldService = worldService;
        }

        public void SendAttack(string direction)
        {
            Weapon weapon = _worldService.getCurrentPlayer().Inventory.Weapon;
            int x = 0;
            int y = 0;
            switch (direction)
            {
                case "right":
                case "east":
                    x = weapon.GetWeaponDistance();
                    break;
                case "left":
                case "west":
                    x = -weapon.GetWeaponDistance();
                    break;
                case "forward":
                case "up":
                case "north":
                    y = +weapon.GetWeaponDistance();
                    break;
                case "backward":
                case "down":
                case "south":
                    y = -weapon.GetWeaponDistance();
                    break;
            }

            var currentPlayer = _worldService.getCurrentPlayer();
            AttackDTO attackDto = new AttackDTO();
            attackDto.XPosition = currentPlayer.XPosition + x;
            attackDto.YPosition = currentPlayer.YPosition + y;
            attackDto.Damage = weapon.GetWeaponDamage();
            attackDto.Stamina = currentPlayer.Stamina;
            attackDto.PlayerGuid = _clientController.GetOriginId();
            SendAttackDTO(attackDto);
        }

        public void SendAttackDTO(AttackDTO attackDto)
        {
            var payload = JsonConvert.SerializeObject(attackDto);
            _clientController.SendPayload(payload, PacketType.Attack);
        }

        public HandlerResponseDTO HandlePacket(PacketDTO packet)
        {
            AttackDTO attackDto = JsonConvert.DeserializeObject<AttackDTO>(packet.Payload);

            if (_clientController.IsHost() && packet.Header.Target.Equals("host"))
            {
                var allPlayers = _worldService.getAllPlayers();
                var PlayerResult =
                    allPlayers.Where(x =>
                        x.XPosition == attackDto.XPosition && x.YPosition == attackDto.YPosition);

                //var CreatureResult =
                //    allCreatures.Where(x =>
                //        x.XPosition == attackDto.XPosition && x.YPosition == attackDto.YPosition &&
                //        x.GameGuid == _clientController.SessionId);

                InsertStaminaToDatabase(attackDto);

                if (PlayerResult.Any())
                {
                    attackDto.AttackedPlayerGuid = PlayerResult.FirstOrDefault().Id;
                    if (attackDto.Stamina >= ATTACKSTAMINA)
                    {
                        InsertDamageToDatabase(attackDto, true);
                        packet.Payload = JsonConvert.SerializeObject(attackDto);
                    }
                    else
                    {
                        Console.WriteLine("Insufficient stamina to perform attack action.");
                    }
                }
                //else if (CreatureResult.Any())
                //{
                //    attackDto.AttackedPlayerGuid = CreatureResult.FirstOrDefault().CreatureGuid;
                //    InsertDamageToDatabase(attackDto, false);
                //    packet.Payload = JsonConvert.SerializeObject(attackDto);
                //}
                else
                {
                    if (_clientController.GetOriginId().Equals(attackDto.PlayerGuid))
                    {
                        HandleAttack(attackDto);
                        Console.WriteLine("There is no enemy to attack");
                    }

                    return new HandlerResponseDTO(SendAction.ReturnToSender,
                        "There is no enemy to attack");
                }
            }
            else if (packet.Header.Target.Equals(_clientController.GetOriginId()))
            {
                Console.WriteLine(packet.HandlerResponse.ResultMessage);
            }

            HandleAttack(attackDto);
            return new HandlerResponseDTO(SendAction.SendToClients, null);
        }

        private void InsertStaminaToDatabase(AttackDTO attackDto)
        {
            var dbConnection = new DbConnection();
            var playerRepository = new Repository<PlayerPOCO>(dbConnection);
            var player = playerRepository.GetAllAsync().Result
                .FirstOrDefault(player => player.PlayerGuid == attackDto.PlayerGuid);
            player.Stamina -= ATTACKSTAMINA;
            playerRepository.UpdateAsync(player);
        }

        private void InsertDamageToDatabase(AttackDTO attackDto, Boolean isPlayer) // both armor and health damage
        {
            if (isPlayer)
            {
                var dbConnection = new DbConnection();

                var attackedPlayerRepository = new Repository<PlayerPOCO>(dbConnection);
                var attackedPlayer = attackedPlayerRepository.GetAllAsync().Result
                    .FirstOrDefault(attackedPlayer => attackedPlayer.PlayerGuid == attackDto.AttackedPlayerGuid);

                var attackedPlayerItemRepository = new Repository<PlayerItemPOCO>(dbConnection);
                //PlayerItemPOCO attackedPlayerItem;

                if (_worldService.getCurrentPlayer().Inventory.Helmet != null)
                {
                    var attackedPlayerHelmet = _worldService.getCurrentPlayer().Inventory.Helmet;
                    var helmetPoints = attackedPlayerHelmet.ArmorProtectionPoints;

                    var attt = attackedPlayerItemRepository.GetAllAsync();
                   var attackedPlayerItem = attackedPlayerItemRepository.GetAllAsync().Result
                        .FirstOrDefault(attackedPlayerItem => attackedPlayerItem.PlayerGUID == attackDto.PlayerGuid &&
                                                              attackedPlayerItem.ItemName ==
                                                              attackedPlayerHelmet.ItemName);

                    if (helmetPoints - attackDto.Damage <= 0 && helmetPoints != 0)
                    {
                        attackDto.Damage -= attackedPlayerItem.ArmorPoints;
                        attackedPlayerItemRepository.DeleteAsync(attackedPlayerItem);
                    }
                    else
                    {
                        attackedPlayerItem.ArmorPoints -= attackDto.Damage;
                        attackedPlayerItemRepository.UpdateAsync(attackedPlayerItem);
                    }
                }

                if (_worldService.getCurrentPlayer().Inventory.Armor != null)
                {
                    var attackedPlayerBodyArmor = _worldService.getCurrentPlayer().Inventory.Armor;
                    var bodyArmorPoints = attackedPlayerBodyArmor.ArmorProtectionPoints;
                    var attackedPlayerItem = attackedPlayerItemRepository.GetAllAsync().Result
                        .FirstOrDefault(attackedPlayerItem => attackedPlayerItem.PlayerGUID == attackDto.PlayerGuid &&
                                                              attackedPlayerItem.ItemName ==
                                                              attackedPlayerBodyArmor.ItemName);
                    if (bodyArmorPoints - attackDto.Damage <= 0 && bodyArmorPoints != 0)
                    {
                        attackedPlayerItemRepository.DeleteAsync(attackedPlayerItem);
                    }
                    else
                    {
                        attackedPlayerItem.ArmorPoints -= attackDto.Damage;
                        attackedPlayerItemRepository.UpdateAsync(attackedPlayerItem);
                        
                        attackedPlayer.Health -= attackDto.Damage;
                        attackedPlayerRepository.UpdateAsync(attackedPlayer);
                    }
                }
                else
                {
                    attackedPlayer.Health -= attackDto.Damage;
                    attackedPlayerRepository.UpdateAsync(attackedPlayer);
                }
            }
            else
            {
                var dbConnection = new DbConnection();

                var attackedCreatureRepository = new Repository<CreaturePOCO>(dbConnection);
                var attackedCreature = attackedCreatureRepository.GetAllAsync().Result
                    .FirstOrDefault(attackedCreature => attackedCreature.CreatureGuid == attackDto.AttackedPlayerGuid);
                attackedCreature.Health -= attackDto.Damage;
                attackedCreatureRepository.UpdateAsync(attackedCreature);

                if (attackedCreature.Health <= 0)
                {
                    Console.WriteLine("RIP"); //TODO implement death of creature
                }
            }
        }

        private void HandleAttack(AttackDTO attackDto)
        {
            if (_clientController.GetOriginId().Equals(attackDto.PlayerGuid))
            {
                Console.WriteLine("Your stamina got lowered with " + ATTACKSTAMINA + ".");
                _worldService.getCurrentPlayer().Stamina -= ATTACKSTAMINA;
                Console.WriteLine("stamina: " + _worldService.getCurrentPlayer().Stamina);
            }

            if (_clientController.GetOriginId().Equals(attackDto.AttackedPlayerGuid))
            {
                Console.WriteLine("You took a total of: " + attackDto.Damage + " damage.");
                int ArmorPoints = 0;
                int HelmetPoints = 0;
                if (_worldService.getCurrentPlayer().Inventory.Armor != null)
                {
                    ArmorPoints = _worldService.getCurrentPlayer().Inventory.Armor.ArmorProtectionPoints;
                }

                if (_worldService.getCurrentPlayer().Inventory.Helmet != null)
                {
                    HelmetPoints = _worldService.getCurrentPlayer().Inventory.Helmet.ArmorProtectionPoints;
                }

                //Eerst wordt Damage van de helm afgehaald, vervolgens van de body armor en tot slot van de speler.
                if (HelmetPoints - attackDto.Damage <= 0 && HelmetPoints != 0)
                {
                    Console.WriteLine("Your helmet has been destroyed!");
                    attackDto.Damage -= HelmetPoints;
                    _worldService.getCurrentPlayer().Inventory.Helmet = null;
                }
                else if (HelmetPoints != 0)
                {
                    Console.WriteLine("Your Helmet took " + attackDto.Damage + " damage.");
                    attackDto.Damage = 0;
                    _worldService.getCurrentPlayer().Inventory.Helmet.ArmorProtectionPoints -= attackDto.Damage;
                }

                if (ArmorPoints - attackDto.Damage <= 0 && ArmorPoints != 0)
                {
                    Console.WriteLine("Your armor piece has been destroyed!");
                    attackDto.Damage -= ArmorPoints;
                    _worldService.getCurrentPlayer().Inventory.Armor = null;
                    _worldService.getCurrentPlayer().Health -= attackDto.Damage;
                    Console.WriteLine("Your health took " + attackDto.Damage + " damage.");
                }
                else if (ArmorPoints != 0)
                {
                    Console.WriteLine("Your armor took " + attackDto.Damage + " damage.");
                    _worldService.getCurrentPlayer().Inventory.Armor.ArmorProtectionPoints -= attackDto.Damage;
                }
                else
                {
                    Console.WriteLine("Your health took " + attackDto.Damage + " damage.");
                    _worldService.getCurrentPlayer().Health -= attackDto.Damage;
                }

                if (_worldService.getCurrentPlayer().Health <= 0)
                {
                    Console.WriteLine("isded"); //TODO implement death of Player. Boolean isDead in DB?
                }
            }
        }
    }
}