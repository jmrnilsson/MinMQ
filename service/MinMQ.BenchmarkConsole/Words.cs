using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MinMQ.BenchmarkConsole
{
	public class Words
	{
		private int wordIndex = 0;
		private string[] words = WordFactory();

		public static string[] WordFactory()
		{
			static IEnumerable<string> GetWords()
			{
				var phrase = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris eleifend leo ut velit pellentesque
				dictum. In venenatis ipsum non lacinia luctus. Aliquam pretium mauris nec quam imperdiet, in euismod ligula
				mollis. Integer risus diam, facilisis at tellus eget, fringilla hendrerit orci. Pellentesque at ornare diam.
				Quisque semper gravida erat. Sed feugiat felis vel massa consectetur suscipit nec bibendum ligula. Nullam
				pulvinar metus quis porta ornare. Pellentesque sodales nisi sit amet rutrum faucibus. Vestibulum pulvinar
				rhoncus purus, ut bibendum ante pretium in. Suspendisse potenti.

				Vestibulum facilisis ullamcorper lacus id iaculis. Nunc laoreet odio non nisi vulputate fringilla. Etiam erat
				nisi, viverra ut aliquam ac, dignissim eget felis. Integer sit amet augue suscipit, dignissim turpis semper,
				consectetur leo. Phasellus venenatis, libero ac pulvinar facilisis, elit augue ullamcorper ante, quis egestas mi
				justo ut dolor. Fusce nec diam nec est pulvinar faucibus. Vestibulum ante ipsum primis in faucibus orci luctus
				et ultrices posuere cubilia Curae; Morbi sit amet condimentum mi. Pellentesque habitant morbi tristique senectus
				et netus et malesuada fames ac turpis egestas.

				In rutrum ultrices sapien et venenatis. Nulla ornare euismod lectus ac facilisis. Curabitur imperdiet dignissim
				massa quis congue. Ut varius, dui et ornare finibus, lacus ligula porttitor ex, vel fringilla nisl risus et
				odio. Morbi ultricies volutpat mauris non bibendum. Aliquam at arcu sed massa venenatis sodales vitae nec ipsum.
				Suspendisse euismod lobortis massa eu molestie. Vivamus quis erat quis erat sollicitudin tristique. Praesent
				ornare consequat ipsum vel porta. Vivamus dignissim at diam sed faucibus.

				Duis nisi purus, lacinia eget magna vel, cursus ultricies erat. Suspendisse id sapien ullamcorper, maximus nisi
				ut, sodales dolor. Aenean consequat, est vitae venenatis rutrum, est massa dapibus magna, ac faucibus mauris
				augue eget purus. Suspendisse vel eros sapien. Curabitur vel sem eu sapien suscipit commodo. Vivamus vulputate
				nisl et ligula aliquet, nec dapibus lacus hendrerit. Etiam lobortis ornare nulla rutrum dignissim. Nulla
				sollicitudin non ipsum id ullamcorper. Nulla facilisi.

				Cras neque diam, dapibus eu felis sit amet, sollicitudin pellentesque diam. Ut et tincidunt dolor, vel varius
				urus. Etiam ut nibh sit amet tellus vestibulum pulvinar. Phasellus ante felis, venenatis interdum tempor eu,
				ornare vitae mi. Duis porttitor ipsum ac sapien dictum, in scelerisque lacus imperdiet. Nullam venenatis
				pellentesque elit a volutpat. Sed hendrerit tristique felis nec sagittis. Quisque in diam vulputate, porta purus
				nec, viverra mauris. Morbi accumsan consectetur lectus, eget semper sapien sagittis in. Donec quam lacus,
				consequat semper nunc eu, suscipit scelerisque magna. Curabitur porta arcu nec iaculis convallis. Duis mattis
				tempor mi tristique commodo. Pellentesque nec consequat dolor. ";

				foreach (Match match in Regex.Matches(phrase, "\\w+"))
				{
					yield return match.Value;
				}
			}

			return GetWords().ToArray();
		}

		public string Pick()
		{
			if (++wordIndex > 0 && wordIndex % words.Length == 0)
			{
				wordIndex = 0;
			}

			return words[wordIndex];
		}
	}
}
