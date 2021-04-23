using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace genetykav2
{
    
    class Program
    {
       static DateTime localDate = DateTime.Now;
        static GeneticAlgorithm<char> ga;
        static void Main(string[] args)
        {
            FileStream fileStream = File.Open("log.txt", FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close(); 

                Random random = new Random();
            
            int populationSize = 200;
            float mutationRate = 0.01f;
            int elitism = 1;
            string targetString = "Sample string to be replaced";
            string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUWXYZ,.|!@#$%^&*()_+=? ";

            Console.WriteLine("podaj rozmiar populacji");
            populationSize =Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Podaj szanse na mutacje eg.: 0.03");
            mutationRate = float.Parse(Console.ReadLine());

            Console.WriteLine("Podaj idealne geny końcowe w formie teksowej eg: To be or not to be!");
            Console.WriteLine("Zdanie nie może zawierać polskich znaków");
            targetString = Console.ReadLine();



            Console.WriteLine("Podaj poziom elitismu (zakres od 0 do 100)");
            elitism = Convert.ToInt32(Console.ReadLine());

            
            Console.WriteLine("Podaj liczbę iteracji, 0 jeżeli ma dążyć do pokolenia z najlepszymi możliwymi genami");
            int howlong = Convert.ToInt32(Console.ReadLine());
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                w.WriteLine("Algorytm wykonany: {0} Rozmiar populacji: {1} szansa na mutacje: {2}% elitism: {3}% Geny docelowe: {4}", 
                    localDate.ToString(),
                    populationSize.ToString(),
                    mutationRate.ToString(),
                    elitism.ToString(),
                    targetString.ToString()
                    
                    );
                w.Close();
            }
            ga = new GeneticAlgorithm<char>(
                populationSize,
                targetString.Length,
                random,
                GetrandomGene,
                fitnessFunction: FitnessFunction,
                elitism,
                mutationRate: mutationRate);
            string text="";
            if (howlong == 0)
            {
                while (text != targetString)
                {
                    ga.NewGeneration();
                    Console.WriteLine(ga.Generation);
                    Console.WriteLine(ga.BestGenes);
                     text = new string(ga.BestGenes);
                    using (StreamWriter w = File.AppendText("log.txt"))
                    {
                        w.WriteLine("{0} Najlepsze geny w populacji: {1} Fitness:{2}",
                           ga.Generation.ToString(),
                           text,
                           ga.BestFitness);
                    }
                }
            }
            else
            {
                for (int i = 0; i < howlong; i++)
                {
                    ga.NewGeneration();
                    Console.WriteLine(ga.Generation);
                    Console.WriteLine(ga.BestGenes);
                    text = new string(ga.BestGenes);
                    using (StreamWriter w = File.AppendText("log.txt"))
                    {
                        w.WriteLine("{0} Najlepsze geny w populacji: {1} Fitness:{2}",
                           ga.Generation.ToString(),
                           text,
                           ga.BestFitness);
                    }
                }
            }
            


             char GetrandomGene()
            {
               
                int i = random.Next(validCharacters.Length);
                return validCharacters[i];
            }

            float FitnessFunction(int index)
            {
                float score = 0;
                DNA<char> DNA;
                DNA = ga.Population[index];
                for (int i = 0; i < DNA.Genes.Length; i++)
                {
                    if (DNA.Genes[i] == targetString[i])
                    {
                        score += 1;
                    }
                }
                score /= targetString.Length;
                return score;
            }
        }

        
    }


    public class DNA<T>
    {
        public T[] Genes { get; private set; }
        public float Fitness { get; private set; }

        private Random random;
        private Func<T> getRandomGene;
        private Func<int, float> fitnessFunction;

        public DNA(int size, Random random, Func<T> getRandomGene, Func<int, float> fitnessFunction, bool shouldInitGenes = true)
        {
            Genes = new T[size];
            this.random = random;
            this.getRandomGene = getRandomGene;
            this.fitnessFunction = fitnessFunction;

            if (shouldInitGenes)
            {
                for (int i = 0; i < Genes.Length; i++)
                {
                    Genes[i] = getRandomGene();
                }
            }
        }

        public float CalculateFitness(int index)
        {
            Fitness = fitnessFunction(index);
            return Fitness;
        }

        public DNA<T> Crossover(DNA<T> otherParent)
        {
            DNA<T> child = new DNA<T>(Genes.Length, random, getRandomGene, fitnessFunction, shouldInitGenes: false);

            for (int i = 0; i < Genes.Length; i++)
            {
                child.Genes[i] = random.NextDouble() < 0.5 ? Genes[i] : otherParent.Genes[i];
            }

            return child;
        }

        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                if (random.NextDouble() < mutationRate)
                {
                    Genes[i] = getRandomGene();
                }
            }
        }
    }


}
