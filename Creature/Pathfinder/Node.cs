﻿/*
    AIM SD ASD 2020/2021 S2 project
     
    Project name: [to be determined].
 
    This file is created by team: 4.
     
    Goal of this file: Nodes maken en klaarzetten voor gebruik in path finding.
     
*/

using System.Numerics;


namespace Creature.Pathfinder
{
    class Node
    {
        public const int nodeSize = 1;
        public Node parent;
        public Vector2 position;
        public Vector2 Center
        {
            get
            {
                return new Vector2(position.X + nodeSize / 2, position.Y + nodeSize / 2);
            }
        }
        public float distanceToTarget;
        public float cost;
        public float weight;
        public float FScore
        {
            get
            {
                if (distanceToTarget != -1 && cost != -1)
                    return distanceToTarget + cost;
                else
                    return -1;
            }
        }
        public bool isWalkable;
        public Node(Vector2 pos, bool isWalkable, float weight = 1)
        {
            this.parent = null;
            this.position = pos;
            this.distanceToTarget = -1;
            this.cost = 1;
            this.weight = weight;
            this.isWalkable = isWalkable;
        }
    }
}
