using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SDH.MoviePlugins
{
    public class MovieCreateGetOmdbApiController
    {
        public MovieCreateGetOmdbApiController()
        {
        }

        public OptionSetValue GetOptionSetValue(string entityName, string attName, string value, IOrganizationService service, ITracingService traceService)
        {
            var result = new OptionSetValue();

            var optionSetValues = Helpers.GetOptionSetCollection(entityName, attName, service, traceService);

            if (optionSetValues.ContainsKey(value))
            {
                result = new OptionSetValue(optionSetValues[value].Value);
            }
            else
            {
                throw new InvalidPluginExecutionException($"Choice {value} does not exist.");
            }
            return result;

        }

        public String GetMovieRecord(string title, string year, string omdbApiKey)
        {
            var result = string.Empty;
            var url = GetOmdbApiUrlString(title, year, omdbApiKey);
            using (CustomWebClient client = new CustomWebClient())
            {
                byte[] responseBytes = client.DownloadData(url);
                result = Encoding.UTF8.GetString(responseBytes);
            }
            return result;
            //return "{\"Title\":\"Blade Runner\",\"Year\":\"1982\",\"Rated\":\"R\",\"Released\":\"25 Jun 1982\",\"Runtime\":\"117 min\",\"Genre\":\"Action, Sci-Fi, Thriller\",\"Director\":\"Ridley Scott\",\"Writer\":\"Hampton Fancher (screenplay), David Webb Peoples (screenplay), Philip K. Dick (novel)\",\"Actors\":\"Harrison Ford, Rutger Hauer, Sean Young, Edward James Olmos\",\"Plot\":\"A blade runner must pursue and terminate four replicants who stole a ship in space, and have returned to Earth to find their creator.\",\"Language\":\"English, German, Cantonese, Japanese, Hungarian, Arabic\",\"Country\":\"USA\",\"Awards\":\"Nominated for 2 Oscars. Another 12 wins & 17 nominations.\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BNzQzMzJhZTEtOWM4NS00MTdhLTg0YjgtMjM4MDRkZjUwZDBlXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_SX300.jpg\",\"Ratings\":[{\"Source\":\"Internet Movie Database\",\"Value\":\"8.1/10\"},{\"Source\":\"Rotten Tomatoes\",\"Value\":\"90%\"},{\"Source\":\"Metacritic\",\"Value\":\"84/100\"}],\"Metascore\":\"84\",\"imdbRating\":\"8.1\",\"imdbVotes\":\"695,428\",\"imdbID\":\"tt0083658\",\"Type\":\"movie\",\"DVD\":\"15 Nov 2016\",\"BoxOffice\":\"$32,868,943\",\"Production\":\"Blade Runner Partnership, Ladd Company\",\"Website\":\"N/A\",\"Response\":\"True\"}";
        }

        public Entity ConvertToEntity(OmdbResponseData response, Entity entity, IOrganizationService service, ITracingService traceService)
        {
            entity["sdh_jsonresponse"] = response.Json;
            entity["sdh_awards"] = response.Awards;
            entity["sdh_imdbid"] = response.imdbID;
            entity["sdh_plot"] = response.Plot;
            entity["sdh_runtime"] = response.Runtime;
            entity["sdh_poster"] = response.Poster;

            entity["sdh_actors"] = response.Actors;
            entity["sdh_writers"] = response.Writer;
            entity["sdh_directors"] = response.Director;

            if (!response.Website.Equals("N/A"))
            {
                entity["sdh_website"] = response.Website;
                traceService.Trace($"sdh_website has value {response.Website}");
            }
            if (!response.BoxOffice.Equals("N/A"))
            {
                entity["sdh_boxoffice"] = new Money(Convert.ToDecimal(response.BoxOffice.Replace("$", "").Replace(",", "")));
                traceService.Trace($"sdh_boxoffice has value {response.BoxOffice}");
            }

            if (!response.DVD.Equals("N/A"))
            {
                entity["sdh_dvd"] = Convert.ToDateTime(response.DVD);
                traceService.Trace($"sdh_dvd has value {response.DVD}");
            }

            if (!response.Released.Equals("N/A"))
            {
                entity["sdh_released"] = Convert.ToDateTime(response.Released);
                traceService.Trace($"sdh_released has value {response.Released}");
            }

            if (!response.imdbRating.Equals("N/A"))
            {
                entity["sdh_imdbrating"] = response.imdbRating;
                traceService.Trace($"sdh_imdbrating has value {response.imdbRating}");
            }

            if (!response.imdbVotes.Equals("N/A"))
            {
                entity["sdh_imdbvotes"] = Convert.ToInt32(response.imdbVotes.Replace(",", ""));
                traceService.Trace($"sdh_imdbVotes has value {response.imdbVotes}");
            }

            if (!response.Metascore.Equals("N/A"))
            {
                entity["sdh_metascore"] = response.Metascore;
                traceService.Trace($"sdh_metascore has value {response.Metascore}");
            }

            if (!response.Country.Equals("N/A"))
            {
                //keep it as choices 
                var countryArray = response.Country.Split(',');
                var countryList = new List<string>();

                foreach (var country in countryArray)
                {
                    countryList.Add(country.Trim());
                }
                entity["sdh_country"] = GetOptionSetValuesCollection("sdh_movie", "sdh_country", countryList, service, traceService);
            }

            if (!response.Genre.Equals("N/A"))
            {
                var genreArray = response.Genre.Split(',');
                var genreList = new List<string>();

                foreach (var genre in genreArray)
                {
                    genreList.Add(genre.Trim());
                }
                entity["sdh_genre"] = GetOptionSetValuesCollection("sdh_movie", "sdh_genre", genreList, service, traceService);
                traceService.Trace($"sdh_genre has value");
            }
            if (!response.Language.Equals("N/A"))
            {
                var langArray = response.Language.Split(',');
                var langList = new List<string>();

                foreach (var lang in langArray)
                {
                    langList.Add(lang.Trim());
                }
                entity["sdh_language"] = GetOptionSetValuesCollection("sdh_movie", "sdh_language", langList, service, traceService);
                traceService.Trace($"sdh_language has value");
            }
            if (!response.Rated.Equals("N/A"))
            {
                var optionSetValue = GetOptionSetValuesCollection("sdh_movie", "sdh_rated", response.Rated, service, traceService);
                if (optionSetValue != null)
                    entity["sdh_rated"] = optionSetValue;
                traceService.Trace($"sdh_rated has value {optionSetValue}");
            }
            if (!response.Response.Equals("N/A"))
            {
                if (response.Response.ToUpper().Equals("TRUE"))
                {
                    entity["sdh_response"] = true;
                }
                else
                {
                    entity["sdh_response"] = false;
                }
                traceService.Trace($"sdh_response has value {entity["sdh_response"]}");
            }
            if (!response.Type.Equals("N/A"))
            {
                var optionSetValue = GetOptionSetValue("sdh_movie", "sdh_type", response.Type, service, traceService);
                if (optionSetValue != null)
                    entity["sdh_type"] = optionSetValue;
                traceService.Trace($"sdh_type has value {optionSetValue}");
            }
            return entity;
        }

        public OmdbResponseData GetOmdbApiResponse(string movieData)
        {
            var result = new OmdbResponseData();
            using (MemoryStream DeSerializememoryStream = new MemoryStream())
            {
                string ResponseString = movieData;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(OmdbResponseData));

                StreamWriter writer = new StreamWriter(DeSerializememoryStream);
                writer.Write(ResponseString);
                writer.Flush();

                DeSerializememoryStream.Position = 0;
                result = (OmdbResponseData)serializer.ReadObject(DeSerializememoryStream);
            }
            result.Json = movieData;
            return result;
        }

        private string GetOmdbApiUrlString(string title, string year, string key)
        {
            return $"https://www.omdbapi.com/?apikey={key}&t={title}&y={year}";
        }

        public OptionSetValue GetOptionSetValuesCollection(string entityName, string attName, string value, IOrganizationService service, ITracingService traceService)
        {
            var result = new OptionSetValue();
            var optionSetValues = Helpers.GetOptionSetCollection(entityName, attName, service, traceService);
            if (optionSetValues.ContainsKey(value))
            {
                result = new OptionSetValue(optionSetValues[value].Value);
            }
            else
            {
                throw new InvalidPluginExecutionException($"The choice: {value} in the attribute: {attName} in the entity: {entityName}  does not exist.");
            }
            return result;

        }

        public OptionSetValueCollection GetOptionSetValuesCollection(string entityName, string attName, List<string> values, IOrganizationService service, ITracingService traceService)
        {
            var result = new OptionSetValueCollection();
            var optionSetValues = Helpers.GetOptionSetCollection(entityName, attName, service, traceService);

            foreach (var label in values)
            {
                if (optionSetValues.ContainsKey(label))
                {
                    result.Add(new OptionSetValue(optionSetValues[label].Value));
                }
                else
                {
                    throw new InvalidPluginExecutionException($"The option: {label} in the attribute: {attName} in the entity: {entityName} does not exist. Please contact and Admin to Add.");
                }
            }

            return result;

        }

        public string GetAccountByNameFetch(string name)
        {
            return $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='account'>
                        <attribute name='accountid' />
                        <order attribute='name' descending='false' />
                        <filter type='and'>
                          <condition attribute='name' operator='eq' value='{name}' />
                        </filter>
                      </entity>
                    </fetch>";
        }

    }
}