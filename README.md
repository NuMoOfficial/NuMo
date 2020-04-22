# NuMo
This is the official code base and branch for the NUtritional MOnitoring health app
Most current NuMo version as of April 2020

This website holds the PDFs that are used in the mental health page.
https://github.com/NuMoOfficial/NuMoOfficial.github.io

Here are links to the class diagrams as of April 2020
https://drive.google.com/file/d/1Z0mSeHhffwfBuKjHMeVsbWRoslvxv2_4/view?usp=sharing (Main App)
https://drive.google.com/file/d/1St5h2jhNRomN5mc02f_wl_ngfjDYDvPG/view?usp=sharing (DataAccessor and Memento pattern)
https://drive.google.com/file/d/1EQ6U8b2g6EYpClr9DbxD_rPeLcdJ2icu/view?usp=sharing (Everything)

These class diagrams can be editted with draw.io (aka app.diagrams.net), a free modeling software.
PLEASE ASK ED OR HOLLY TO CONTACT ONE OF THE DEVELOPERS OF THE 2019-2020 NUMO TEAM TO GRANT YOU EDITING ACCESS. 
These class diagrams are currently set to comment and view only.
Or, if you are lucky, you may be able to copy/paste everything into a new draw.io document. I don't know if that is possible when commenting access is available. 
This is to prevent random people from making changes to the diagrams.

Future work:

Ability to edit/remove items in a recipe as the user is creating the recipe (looking at HomePage.xaml and HomePage.xaml.cs may be helpful)

Ability to add/edit/remove items in a recipe that has already been user created

Ability to edit/remove foods that have already been user created

I was able to crash the add recipe page by entering -0 into the quantity field. I was also able to successfully enter a negative number into the quantity field. 

Scan grocery store receipts and link to database (keep seperate from daily food intake). Output nutrients as a visual to see gross amount of macro/micro nutrients? Ask Ed and Holly about this one. If they want you to attempt this, you may want to create an informational gross analysis of macro/micronutrients for purchased items. 

Try getting Firebase Crashalytics working for Android. When we tried using it, the app crashed on startup. There is a line commented out in NuMoTabbed.Android/MainActivity.cs //Fabric.Fabric.With(this, new Crashlytics.Crashlytics());




