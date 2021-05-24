# MovieDatabaseD365

The underlying database in this project is a model driven app using the Power Apps platform to store movies and related records: movie ratings, movie production companies, and movie roles. 

The model driven app consists of out of the box solution entities as well custom entities and is used as the underlying data model in order to create the canvas app. 

Through the use of a plugin and the event: creation of a movie, the database queries the omdb api through a plugin given a movie title and year, which will populate fields. On a workflow triggered by movie creation, several actions are called in each step to create related records. 

In the first action, the inputs are the movie entityreference, and the movie roles as a comma delimited string value, and the roles associated with the movie are created. In the next action, the inputs are the movie entityreference and the json response of the omdb api. This action creates the movies ratings records. Lastly, the inputs of the last action are the movie entityreference and the json response of the omdb api, which is used to create the movie production company records and associate them with the movie.

In all three actions, code from different custom Plugins are triggered to find existing and create new associated records. 

SDHMovieDatabase_1_0_0_6.zip is the unmanaged solution, while the directory SDH.MoviePlugins contains the plugin code. 


