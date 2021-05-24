using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace SDH.MoviePlugins
{
    public class CreateMovieRolesController
    {
        public string Actors { get; set; }
        public string Writers { get; set; }
        public string Directors { get; set; }
        public IOrganizationService Service { get; set; }
        public ITracingService Trace { get; set; }
        public Dictionary<string, int?> RolesOptionSetValues { get; set; } 
        public CreateMovieRolesController(string actors, string writers, string directors, IOrganizationService service, ITracingService trace)
        {
            Actors = actors;
            Writers = writers;
            Directors = directors;
            Service = service;
            Trace = trace;
            RolesOptionSetValues = Helpers.GetOptionSetCollection("sdh_role", "sdh_position", this.Service, this.Trace);
        }

        public void Run(EntityReference movie, string movieTitle)
        {
            this.Trace.Trace("inside Run of controller!!");
            var directorsValue = this.Directors;

            this.Trace.Trace(directorsValue);

            var writersValue = this.Writers;

            Trace.Trace(writersValue);

            var actorsValue = this.Actors;

            this.Trace.Trace(actorsValue);

            this.Trace.Trace($"Movie ID is {movie.Id}");

            var roles = new Dictionary<string, List<Role>>();

            var actors = GetValues(actorsValue);
            var directors = GetValues(directorsValue);
            var writers = GetValues(writersValue);
            roles = AssignRoles(actors, "Actor", roles, movie);
            roles = AssignRoles(directors, "Director", roles, movie);
            roles = AssignRoles(writers, "Writer", roles, movie);
           
            foreach (var person in roles)
            {
                List<Role> rolesList = person.Value;
                var people = this.Service.RetrieveMultiple(new FetchExpression(GetPeopleByNameFetchXml(person.Key)));
                EntityReference entityRef;
                if (people.Entities.Count == 0)
                {
                    var personEntity = new Entity("sdh_person");
                    personEntity["sdh_name"] = person.Key;
                    personEntity.Id = this.Service.Create(personEntity);
                    entityRef = new EntityReference("sdh_person", personEntity.Id);
                } 
                else
                {
                    entityRef = new EntityReference("sdh_person", people.Entities[0].Id);
                }
                Trace.Trace($"EntityId: {entityRef.Id}");
                foreach (var role in rolesList)
                {
                    role.Person = entityRef;
                    var roleEntity = new Entity("sdh_role");

                    Trace.Trace($"Position: {role.Position.Value}, Person: {role.Person}, Movie: {role.Movie}");
                    roleEntity["sdh_position"] = role.Position;
                    roleEntity["sdh_person"] = role.Person;
                    roleEntity["sdh_movie"] = role.Movie;
                    roleEntity["sdh_name"] = $"{role.Position} in {role.Movie}";

                    try
                    {
                        var id = this.Service.Create(roleEntity);
                        Trace.Trace($"Created new role record with ID: {id}");
                    }
                    catch (Exception e)
                    {
                        Trace.Trace($"Message: {e.Message}");
                        Trace.Trace($"StackTrace: {e.StackTrace}");
                        Trace.Trace("\n");
                    }
                }
            }
        }

        private Dictionary<string, List<Role>> AssignRoles(List<string> names, string position, Dictionary<string, List<Role>> roles, EntityReference movie)
        {
            foreach (var name in names)
            {
                var value = (int)RolesOptionSetValues[position];
                var newPositionOptionSet = new OptionSetValue(value);
                var newRole = new Role() { 
                    PersonName = name,
                    Movie = movie,
                    Position = newPositionOptionSet
                };
                if (!roles.ContainsKey(name))
                {
                    roles[name] = new List<Role>();
                }
                roles[name].Add(newRole);
            }
            return roles;
        }

        private List<string> GetValues(string peopleValue)
        {
            var valuesUntrimmed = peopleValue.Split(',');
            var values = new List<string>();
            foreach (var value in valuesUntrimmed)
            {
                values.Add(value.Trim());
            }

            return values;
        }

        public string GetPeopleByNameFetchXml(string name)
        {
            return $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='sdh_person'>
                        <attribute name='sdh_personid' />
                        <order attribute='sdh_name' descending='false' />
                        <filter type='and'>
                          <condition attribute='sdh_name' operator='eq' value='{name}' />
                        </filter>
                      </entity>
                    </fetch>";
        }


    }
}
