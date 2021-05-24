using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDH.MoviePlugins;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var controller = new MovieCreateGetOmdbApiController();

            var json = controller.GetMovieRecord("Code 8", "2019", "fc5f535f");
            Console.WriteLine(json);
            var response = controller.GetOmdbApiResponse(json);
            Console.WriteLine($"Value Plot: {response.Plot}");
            Console.WriteLine("");
            foreach (var rating in response.Ratings)
            {
                Console.WriteLine($"Source: {rating.Source}, Value: {rating.Value}");
            }

            Console.ReadKey();
        }
    }
}
