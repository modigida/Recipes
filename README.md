# Recipes

This is a WPF application built with the MVVM design pattern. The project uses a **SQL Server database** and **Entity Framework** for data management.  

<img src="Recipes/Images/HomePage.png" alt="Image of recipe list" width="600" >

## Purpose  

The application serves as a digital recipe book where users can:  
- Add, edit, and delete recipes.

<img src="Recipes/Images/DetailedRecipe.png" alt="Image of detailed recipe" width="600" >
  
- Sort the recipe list by:  
  - Preparation time  
  - Favorite recipes  
  - Recipe name
 
<img src="Recipes/Images/OrderBy.png" alt="Image of order by dropdown" width="200" >

- Manage ingredients stored in the database.  

<img src="Recipes/Images/IngredientList.png" alt="Image of ingredient list" width="400" >

## Technologies Used  

- **WPF (Windows Presentation Foundation)**  
- **C#**  
- **Entity Framework**  
- **SQL Server**  
- **MVVM Design Pattern**  

<img src="Recipes/Images/SelectCookingTime.png" alt="Image of cooking time dropdown" width="200" >

## Running the Application  

1. **Clone the Repository**:  
   Clone this repository to your local machine:  
   ```bash  
   git clone https://github.com/modigida/Recipes.git

<img src="Recipes/Images/RecipeIngredientList.png" alt="Image of ingredients in recipe" width="400" >

2. **Set Up the Database**:  
- Add your own connection string to the project.
- Optionally, restore the provided backup file to your SQL Server using Management Studio (SSMS) to use the same database that was used during development.

<img src="Recipes/Images/AddIngredientSuggestion.png" alt="Image of suggestions when adding ingredient in recipe" width="200" >

3. **Run the Application**:  
Open the solution in Visual Studio, build the project, and run the application.

## Features
- A user-friendly interface for managing recipes and ingredients.
- Sorting functionality for recipes to easily find favorites, quickest to prepare, or alphabetical order.
- Fully integrated with SQL Server to store and retrieve recipe and ingredient data.

<img src="Recipes/Images/RecipeTags.png" alt="Image of recipe tags" width="400" >
