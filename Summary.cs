using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;


namespace Summarizer
{
	public class Summary
	{
		string[] articleSenteces;
		Dictionary<string, double> finalScores = new Dictionary<string, double>();
		double[,] sentenceScores;
		private string article= "";

		string[] exclusionList = {"the", "The", "and", "a","A", "with", "without", "of", "on","On", "in", "In", "is", "are", "to"};

		public Summary()
		{
		}
		public void setArticle(string text)
		{
			article = text;
		}
		public string getArticle()
		{
			return article;
		}
		public void decomposeArticle()
		{
			articleSenteces = Regex.Split(getArticle(), @"(?<=[\.!\?])\s+");
		}
		private void fillScores() 
		{
			sentenceScores = new double[articleSenteces.Length, articleSenteces.Length];
			double sum = 0;
			for(int i = 0; i < articleSenteces.Length; i++)
			{
				for(int j = 0; j < articleSenteces.Length; j++)
				{
					if(i == j)
						sentenceScores[i,j] = 0;
					else
					sentenceScores[i, j] = intersectionScore(articleSenteces[i], articleSenteces[j]);
				}
			}
			for(int i = 0; i < articleSenteces.Length; i++)
			{
				for(int j = 0; j < articleSenteces.Length; j++)
				{
					sum += sentenceScores[i, j];
				}
				try {
					finalScores.Add(articleSenteces[i], sum);
				} catch {
					Console.WriteLine(sum + "");
				}
				sum = 0;
			}
		}
		public double intersectionScore(string sentence1, string sentence2)
		{
			double count = 0;
            bool exclude = false;

			string[] sentence2Words = Regex.Split(sentence2, @"\W+");
            //Split second sentence
            //Calculate Jaro-wrinkler distance between the two
            //sum the distances to get score
			for(int i = 0; i < sentence2Words.Length; i++)
			{
                for (int x = 0; x < exclusionList.Length; x++)
                    if (sentence2Words[i] == exclusionList[x])
                        exclude = true;

                    if(!exclude)
				        count += Regex.Matches(sentence1, sentence2Words[i]).Count;
			}
			return count / (0.5*(sentence1.Length + sentence2.Length));
		}
		public void printScores()
		{
            string summary = null;
			foreach (KeyValuePair<string, double> pair in finalScores)
				{
				    summary += pair.Key + " " + pair.Value + "\n";
				}

		}
        public string getSummarySentences(int summaryDensity)
        {
            string finalString = null;
            finalScores = finalScores.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            string[] sent = new string[finalScores.Count];
            int i = 0;
            foreach (KeyValuePair<string, double> pair in finalScores)
            {
                sent[i] = pair.Key;
                i++;
            }
            for (int x = sent.Length-1; x >= sent.Length-summaryDensity; x--)
                finalString += sent[x] + "\n\n";
            return finalString;
        }
        public void Summarize()
		{
			decomposeArticle();
			fillScores();
			printScores();
		}
        public void reset()
        {
            articleSenteces = null;
            finalScores = null;
            sentenceScores = null;
        }


	}
}
