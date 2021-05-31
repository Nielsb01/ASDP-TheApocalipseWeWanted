﻿using Creature.Creature.StateMachine.Event;
using Creature.Pathfinder;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Creature.World
{
    public class DefaultWorld : IWorld
    {
        private List<ICreature> _creatures;
        private List<ICreature> _players;
        private List<List<Node>> _nodes;
        private int _size;

        public List<ICreature> Creatures => Creatures;

        public List<ICreature> Players => Players;

        public List<List<Node>> Nodes => _nodes;
        public int Size => _size;

        public DefaultWorld(int initialSize)
        {
            _size = initialSize;
            _nodes = new List<List<Node>>();
            _creatures = new List<ICreature>();
            _players = new List<ICreature>();
        }

        public void GenerateWorldNodes()
        {
            for (int row = 0; row < _size; row++)
            {
                List<Node> nodePoints = new List<Node>();
                for (int col = 0; col < _size; col++)
                {
                    Vector2 nodeLocation = new Vector2(row, col);
                    Node node = new Node(nodeLocation, true);
                    nodePoints.Add(node);
                }
                _nodes.Add(nodePoints);
            }
        }

        public void SpawnAgent(ICreature agent)
        {
            _creatures.Add(agent);
        }

        public void SpawnCreature(ICreature creature)
        {
            _creatures.Add(creature);
        }

        public void SpawnPlayer(ICreature player)
        {
            _players.Add(player);
        }

        public void Render()
        {
            foreach (ICreature creature in _creatures)
            {
                ICreature player = _players[0];
                int attackRange = 3;
                int visionRange = creature.CreatureStateMachine.CreatureData.VisionRange;

                // TODO: implement this using List<Setting>
                //foreach (var block in creature.CreatureStateMachine.CreatureData.RuleSet)
                //{
                //    if (block.ContainsKey("combat") && block["combat"] == "offensive")
                //    {
                //        visionRange += 6;
                //        attackRange += 3;
                //    }
                //}

                if (Vector2.DistanceSquared(creature.CreatureStateMachine.CreatureData.Position, player.CreatureStateMachine.CreatureData.Position) <= visionRange)
                {
                    Vector2 monsterPosition = creature.CreatureStateMachine.CreatureData.Position;
                    Vector2 playerPosition = player.CreatureStateMachine.CreatureData.Position;

                    if (Vector2.Distance(creature.CreatureStateMachine.CreatureData.Position, player.CreatureStateMachine.CreatureData.Position) <= attackRange)
                    {
                        creature.CreatureStateMachine.FireEvent(CreatureEvent.Event.CREATURE_IN_RANGE, player.CreatureStateMachine.CreatureData);
                    }
                    else
                    {
                        creature.CreatureStateMachine.FireEvent(CreatureEvent.Event.SPOTTED_CREATURE, player.CreatureStateMachine.CreatureData);
                    }
                }
                else
                {
                    creature.CreatureStateMachine.FireEvent(CreatureEvent.Event.LOST_CREATURE, player.CreatureStateMachine.CreatureData);
                }
            }

            for (int y = _size; y > 0; y--) {
                string line = null;

                for (int x = 0; x < _size; x++)
                {
                    bool addedLine = false;
                    ICreature player = _players[0];
                    
                    if (player.CreatureStateMachine.CreatureData.Position.X == x && player.CreatureStateMachine.CreatureData.Position.Y == y)
                    {
                        line += "+";
                        addedLine = true;
                    }
                    foreach (ICreature creature in _creatures)
                    {
                        if (creature.CreatureStateMachine.CreatureData.Position.X == x && creature.CreatureStateMachine.CreatureData.Position.Y == y)
                        {
                            line += "|";
                            addedLine = true;
                        }
                    }
                    if (!addedLine) line += "-";
                }
                Console.WriteLine(line);
            }
        }
    }
}
