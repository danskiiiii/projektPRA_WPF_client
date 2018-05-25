## projektPRA_WPF_client ##
This WPF desktop app is part of university project for a programming class "PRAcownia Programowania". 

General idea was to create a client with CRUD functionality, 
communicating via REST API with a server connected to external database.

More detailed info about requirements http://tzietkiewicz.home.amu.edu.pl/?p=85

Link to the ASP.NET server app https://github.com/danskiiiii/projektPRA_ASP.NET_server

### How to run the app ###
You will need Visual Studio. After cloning the repository you'll have to build the solution.
For the client to work correctly, the server part must be online. You'll probably have to edit 
line 87 of MainWindow.Xaml.cs  (client.BaseAddress = new Uri("http://localhost:54940/");) to match
your server address.

### Notes ###
Project is not polished, there are probably a few bugs waiting to happen.
 
UI also could be improved a lot.

Charts work only for Movies, Contracts and Crew Members.

I do not own the rights to the background image.
