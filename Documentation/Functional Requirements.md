<!-- pizzabox::requirements -->
# pizzabox
## architecture
+ [solution] PizzaBox.sln
  + [project - console] PizzaBox.Client.csproj
  + [project - classlib] PizzaBox.Domain.csproj
    + [folder] Abstracts --optional
    + [folder] Interfaces --optional
    + [folder] Models
  + [project - classlib ] PizzaBox.Storing.csproj
    + [folder] Repositories
  + [project - xunit] PizzaBox.Testing.csproj
    + [folder] Mocks --optional
    + [folder] Specs --optional
## requirements
### store
+ [required] should exist at least 1 store
+ [required] should be able to view its completed orders
+ [optional] should be able to view its sales
+ [optional] should be able to view its inventory
+ [optional] should be able to view its users
### order
+ [required] should be able to view its pizzas
+ [required] should be able to compute its cost
+ [required] should be able to limit its cost to no more than $250
+ [optional] should be able to limit its pizza count to no more than 100
### pizza
+ [required] should be able to have a crust
+ [required] should be able to have a size
+ [required] should be able to compute its cost
+ [optional] should be able to have at least 2 default toppings, including cheese and sauce
+ [optional] should be able to limit its toppings to no more than 5
### user
+ [required] should be able to view its order history
+ [required] should be able to only order from 1 location/24-hour period
+ [required] should be able to only order if an account exists
+ [optional] should be able to only order 1 time within a 2 hour period
## technologies
+ .NET Core - C#
+ .NET Core - EF / Serializer
+ .NET Core - xUnit
## timelines
### Jan-22
## showcase (as many as you can implement)
+ [required] as a user i should be able to signin
+ [required] as a user i should be able to view a list of locations
+ [required] as a user i should be able to select a location
+ [required] as a user i should be able to make an order
+ [required] as a user i should be able to choose preset pizza(s)
+ [required] as a user i should be able to select a crust
+ [required] as a user i should be able to select a size
+ [required] as a user i should be able to view my order history
+ [required] as a user i should be able to signout
+ [required] as a store i should be able to view my order history
+ [optional] as a user i should be able to register
+ [optional] as a user i should be able to choose custom pizza(s)
+ [optional] as a user i should be able to select a set of toppings
+ [optional] as a user i should be able to preview my order
+ [optional] as a user i should be able to confirm my order
+ [optional] as a store i should be able to view my sales by day and by month history
