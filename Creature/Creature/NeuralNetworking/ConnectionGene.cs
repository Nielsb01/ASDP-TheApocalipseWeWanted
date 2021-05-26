﻿using MathNet.Numerics.Distributions;
using System;

namespace Creature.Creature.NeuralNetworking
{
    public class ConnectionGene
    {
        public NeuralNode fromNode;
        public NeuralNode toNode;
        public float weight;
        public Boolean enabled = true;
        public int innovationNo;

        public ConnectionGene(NeuralNode from, NeuralNode to, float weight, int inno)
        {
            this.fromNode = from;
            this.toNode = to;
            this.weight = weight;
            this.innovationNo = inno;
        }

        public void MutateWeight()
        {
            var random = new Random();
            float rand2 = random.Next(1);
            if (rand2 < 0.1)
            {//10% of the time completely change the weight
                weight = random.Next(-1, 1);
            }
            else
            {//otherwise slightly change it
                weight += NextGaussian() / 50;
                //keep weight between bounds
                if (weight > 1)
                {
                    weight = 1;
                }
                if (weight < -1)
                {
                    weight = -1;

                }
            }
        }

        private float NextGaussian()
        {
            double mean = 100;
            double stdDev = 10;

            MathNet.Numerics.Distributions.Normal normalDist = new Normal(mean, stdDev);
            float randomGaussianValue = (float)normalDist.Sample();
            return randomGaussianValue;
        }

        //returns a copy of this connectionGene
        public ConnectionGene Clone(NeuralNode from, NeuralNode to)
        {
            ConnectionGene clone = new ConnectionGene(from, to, weight, innovationNo);
            clone.enabled = enabled;

            return clone;
        }
    }
}
