﻿using Agent.antlr.ast;
using Agent.antlr.ast.comparables;
using Agent.antlr.ast.comparables.subjects;

namespace Agent.Tests.generator
{
    public class Fixtures
    {

        public AST GetFixture(string name)
        {
            switch (name) {
                case "test1.txt":
                    return GetTestFixture1();
                case "test2.txt":
                    return GetTestFixture2();
                case "test3.txt":
                    return GetTestFixture3();
                default:
                    return new AST();
            }
        }
        
        
        private AST GetTestFixture1()
        {
            var configuration = new Configuration();

            configuration.AddChild((new Setting("combat"))
                    .AddChild((new Condition())
                        .AddChild((new When())
                            .AddChild(new Player("player"))
                            .AddChild(new Comparison("nearby"))
                            .AddChild(new Player("player"))
                            .AddChild(new ActionReference("attack"))
                        )
                )
            );
            configuration.AddChild((new Setting("combat"))
                .AddChild((new Condition())
                    .AddChild((new When())
                        .AddChild(new Player("player"))
                        .AddChild(new Comparison("nearby"))
                        .AddChild(new Player("player"))
                        .AddChild(new ActionReference("attack"))
                    )
                )
            );
            configuration.AddChild((new Setting("combat"))
                .AddChild((new Condition())
                    .AddChild((new When())
                        .AddChild(new Player("player"))
                        .AddChild(new Comparison("nearby"))
                        .AddChild(new Player("player"))
                        .AddChild(new ActionReference("attack"))
                    )
                )
            );
            return new AST(configuration);
        }
        private static AST GetTestFixture2()
        {
            var configuration = new Configuration();

            configuration.AddChild(new Rule("test", "test"))
                .AddChild(new Rule("test", "test"))
                .AddChild(new Rule("test", "test"));
            configuration.AddChild((new Setting("combat"))
                .AddChild((new Condition())
                    .AddChild((new When())
                        .AddChild(new Player("player"))
                        .AddChild(new Comparison("nearby"))
                        .AddChild(new Player("player"))
                        .AddChild(new ActionReference("attack"))
                    )
                )
                .AddChild((new Action("attack"))
                    .AddChild((new Condition())
                        .AddChild((new When())
                            .AddChild(new Stat("health"))
                            .AddChild(new Comparison("less than"))
                            .AddChild(new Int(50))
                            .AddChild(new ActionReference("flee"))
                            .AddChild((new Otherwise())
                                .AddChild(new ActionReference("engage")))
                        )
                    )
                    )
                .AddChild((new Action("engage"))
                    .AddChild((new Condition())
                        .AddChild((new When())
                            .AddChild(new Inventory("inventory"))
                            .AddChild(new Comparison("contains"))
                            .AddChild(new Item("knife"))
                            .AddChild(new ActionReference("use").AddChild(new Item("knife")))
                        )
                    )
                )
            );
            return new AST(configuration);
        }
        private static AST GetTestFixture3()
        {
            var configuration = new Configuration();
            configuration.AddChild(new Rule("test", "test"))
                .AddChild(new Rule("test", "test"))
                .AddChild(new Rule("test", "test"));
            configuration.AddChild((new Setting("combat"))
                .AddChild((new Condition())
                    .AddChild((new When())
                        .AddChild(new Player("player"))
                        .AddChild(new Comparison("nearby"))
                        .AddChild(new Player("player"))
                        .AddChild(new ActionReference("attack"))
                    )
                )
                .AddChild((new Action("attack"))
                    .AddChild((new Condition())
                        .AddChild((new When())
                            .AddChild(new Stat("health"))
                            .AddChild(new Comparison("less than"))
                            .AddChild(new Int(50))
                            .AddChild(new ActionReference("flee"))
                            .AddChild((new Otherwise())
                                .AddChild(new ActionReference("engage")))
                        )
                    )
                )
                .AddChild((new Action("engage"))
                    .AddChild((new Condition())
                        .AddChild((new When())
                            .AddChild(new Inventory("inventory"))
                            .AddChild(new Comparison("contains"))
                            .AddChild(new Item("knife"))
                            .AddChild(new ActionReference("use").AddChild(new Item("knife")))
                        )
                    )
                )
            );
            
            configuration.AddChild((new Setting("explore"))
                .AddChild((new Condition())
                    .AddChild((new When())
                        .AddChild(new Player("player"))
                        .AddChild(new Comparison("finds"))
                        .AddChild(new Item("item"))
                        .AddChild(new ActionReference("collect"))
                    )
                )
                .AddChild((new Action("collect"))
                    .AddChild((new Condition())
                        .AddChild((new When())
                            .AddChild(new Inventory("inventory"))
                            .AddChild(new Comparison("does not contain"))
                            .AddChild(new Item("item"))
                            .AddChild(new ActionReference("grab"))
                            .AddChild((new Otherwise())
                                .AddChild(new ActionReference("replace")))
                        )
                    )
                )
                .AddChild((new Action("replace"))
                    .AddChild((new Condition())
                        .AddChild((new When())
                            .AddChild(new Item("item").AddChild(new Stat("strength")))
                            .AddChild(new Comparison("greater than"))
                            .AddChild(new Current("current"))
                            .AddChild(new ActionReference("swap"))
                        )
                    )
                )
            );
            
            
            
            return new AST(configuration);
        }
        
        
        
    }
}