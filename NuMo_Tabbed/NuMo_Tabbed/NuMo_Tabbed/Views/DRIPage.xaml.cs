using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NuMo_Tabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DRIPage : ContentPage
    {
        private int ageNum = 0;
        private int gender; //0=female, 1=male
        private int pregnant; //0=false, 1=true
        private int lactating; //0=false, 1=true

        //order:
        //<1, Kids 1-3, Kids 4-8, 
        //Males 9-13, Males 14-18, Males 19-30, Males 31-50, Males 51-70, Males >70,
        //Females 9-13, Females 14-18, Females 19-30, Females 31-50, Females 51-70, Females >70,
        //Pregnancy 14-18, Pregnancy 19-30, Pregnancy 31-50,
        //Lactating 14-18, Lactating 19-30, Lactating 31-50,

        //macronutrients
        private String[] macronutrients = { "dri_calories", "dri_totalCarbs", "dri_dietaryFiber", "dri_sugar", "dri_netCarbs", "dri_protein" };

        private String[] totalCarbs = {"95","130","130","130","130","130","130","130","130","130","130","130","130","130","130",
        "175","175","175","210","210","210"};

        private String[] dietaryFiber = {"19","19","26","31","38","38","38","30","30","26","26","25","25","21","21",
            "28","28","28","29","29","29"};

        private String[] netCarbs = {"76","111","104","99","92","92","92","100","100","104","104","105","105",
            "109","109","147","147","147","181","181","181"};

        private String[] protein = {"11","13","19","34","52","56","56","56","56","34","46","46","46","46","46",
        "71","71","71","71","71","71",};

        //vitamins
        private String[] vitamins = {"dri_vitaminA", "dri_vitaminC", "dri_vitaminD", "dri_vitaminE", "dri_vitaminK", "dri_thiamin",
            "dri_riboflavin", "dri_niacin", "dri_vitaminB6", "dri_folate", "dri_vitaminB12", "dri_pantothenicAcid"};

        private String[] vitaminA = {"500", "300", "400", "600", "900","900","900","900","900","600","700","700","700",
            "700","700","750","770","770", "1200","1300", "1300"};

        private String[] vitaminC = {"50", "15","25","45","75","90","90","90","90","45","65","75","75","75",
            "75","80","85","85","115","120","120"};

        private String[] vitaminD = {"10", "15","15","15","15","15","15","15","20","15","15","15","15","15","20",
            "15","15","15","15","15","15"};

        private String[] vitaminE = {"5", "6", "7", "11","15","15","15","15","15","11","15","15","15","15","15",
            "15","15","15","19","19","19"};

        private String[] vitaminK = {"2.5","30","55","60","75","120","120","120","120","60","75","90","90","90","90",
            "75","90","90","75","90","90"};

        private String[] thiamin = {".3",".5",".6",".9","1.2","1.2","1.2","1.2","1.2",".9","1","1.1","1.1","1.1","1.1",
            "1.4","1.4","1.4","1.4","1.4","1.4"};

        private String[] riboflavin = {".4", ".5", ".6", ".9", "1.3","1.3","1.3","1.3","1.3",".9","1","1.1","1.1","1.1","1.1",
            "1.4","1.4","1.4","1.6","1.6","1.6"};

        private String[] niacin = {"4","6","8","12","16","16","16","16","16","12","14","14","14","14","14","18","18",
            "18","17","17","17"};

        private String[] vitaminB6 = {".3",".5",".6","1","1.3","1.3","1.3","1.7","1.7","1","1.2","1.3","1.3","1.5","1.5",
            "1.9","1.9","1.9","2","2","2"};

        private String[] folate = {"80","150","200","300","400","400","400","400","400","300", "400", "400", "400", "400", "400",
        "600","600","600","500","500","500"};

        private String[] vitaminB12 = {".5",".9","1.2","1.8","2.4","2.4","2.4","2.4","2.4", "1.8", "2.4", "2.4", "2.4", "2.4", "2.4",
            "2.6","2.6","2.6","2.8","2.8","2.8"};

        private String[] pantothenicAcid = {"1.8","2","3","4","5","5","5","5","5","4", "5", "5", "5", "5", "5", "6","6",
            "6","7","7","7"};

        //minerals

        private String[] minerals = {"dri_calcium", "dri_iron", "dri_magnesium", "dri_phosphorus", "dri_potassium",
            "dri_sodium", "dri_zinc", "dri_copper", "dri_manganese", "dri_selenium"};

        private String[] calcium = {"260","700","1000","1300","1300","1000","1000","1000","1200","1300","1300","1000","1000",
            "1200","1200","1300","1000","1000","1300","1000","1000"};

        private String[] iron = {"11","7","10","8","11","8","8","8","8","8","15","18","18","8","8","27","27","27","10"
            ,"9","9"};

        private String[] magnesium = {"75","80","130","240","210","410","400","420","420","420","240","360","310","320",
            "320","320","320","400","350","360","360","310","320"};

        private String[] phosphorus = {"275","460","500","1250","1250","700","700","700","700","1250", "1250", "700", "700", "700", "700",
            "1250","700","700","1250","700","700"};

        private String[] potassium = {"700","3000","3800","4500","4700","4700","4700","4700","4700","4500", "4700", "4700", "4700", "4700", "4700",
        "4700", "4700", "4700","5100","5100","5100"};

        private String[] sodium = {"370","1000","1200","1500","1500","1500","1500","1300","1200","1500", "1500", "1500", "1500", "1300", "1200",
        "1500","1500","1500","1500","1500","1500",};

        private String[] zinc = { "3", "3", "5", "8", "11", "11", "11", "11", "11", "8", "9", "8", "8", "8", "8", "12", "11", "11", "13", "12", "12" };

        private String[] copper = {".220",".340",".440",".700",".890",".900",".900",".900",".900",".700", ".890", ".900", ".900", ".900", ".900",
        "1.000","1.000","1.000","1.300","1.300","1.300"};

        private String[] manganese = {".6","1.2","1.5","1.9","2.2","2.3","2.3","2.3","2.3","1.6","1.6","1.8","1.8","1.8",
        "1.8","2","2","2","2.6","2.6","2.6"};

        private String[] selenium = {"20","20","30","40","55","55","55","55","55","40", "55", "55", "55", "55", "55",
        "60","60","60","70","70","70"};

        private int saveNum = 0; //number to save all of the current_properties stuff for visualizers

        public DRIPage()
        {
            InitializeComponent();
            setValues();
        }

        //pull up the saved customized values, or auto-populate recommended values
        public void setValues()
        {
            var db = DataAccessor.getDataAccessor();
            //if it is custom
            if (db.getSettingsItem("custom_dri").Equals("true"))
            {

                //macronutrients
                foreach (var macronutrient in this.macronutrients)
                {
                    this.FindByName<Entry>(macronutrient).Text = db.getDRIValue(macronutrient);
                }

                //vitamins
                foreach (var vitamin in this.vitamins)
                {
                    this.FindByName<Entry>(vitamin).Text = db.getDRIValue(vitamin);
                }

                //minerals
                foreach (var mineral in this.minerals)
                {
                    this.FindByName<Entry>(mineral).Text = db.getDRIValue(mineral);
                }

            }
            //recommended values, customized is false
            else
            {
                setValuesHelper();
            }
        }

        //for code reuse
        public void setValuesHelper()
        {
            var db = DataAccessor.getDataAccessor();
            calculateSaveNum();

            var calories = calculateCalories();
            var sugar = calculateSugar(calories);

            //macronutrients
            this.FindByName<Entry>("dri_calories").Text = calories;
            this.FindByName<Entry>("dri_totalCarbs").Text = totalCarbs[saveNum];
            this.FindByName<Entry>("dri_sugar").Text = sugar;
            this.FindByName<Entry>("dri_dietaryFiber").Text = dietaryFiber[saveNum];
            this.FindByName<Entry>("dri_netCarbs").Text = netCarbs[saveNum];
            this.FindByName<Entry>("dri_protein").Text = protein[saveNum];

            //vitamins
            this.FindByName<Entry>("dri_vitaminA").Text = vitaminA[saveNum];
            this.FindByName<Entry>("dri_vitaminC").Text = vitaminC[saveNum];
            this.FindByName<Entry>("dri_vitaminD").Text = vitaminD[saveNum];
            this.FindByName<Entry>("dri_vitaminE").Text = vitaminE[saveNum];
            this.FindByName<Entry>("dri_vitaminK").Text = vitaminK[saveNum];
            this.FindByName<Entry>("dri_thiamin").Text = thiamin[saveNum];
            this.FindByName<Entry>("dri_riboflavin").Text = riboflavin[saveNum];
            this.FindByName<Entry>("dri_niacin").Text = niacin[saveNum];
            this.FindByName<Entry>("dri_vitaminB6").Text = vitaminB6[saveNum];
            this.FindByName<Entry>("dri_folate").Text = folate[saveNum];
            this.FindByName<Entry>("dri_vitaminB12").Text = vitaminB12[saveNum];
            this.FindByName<Entry>("dri_pantothenicAcid").Text = pantothenicAcid[saveNum];

            //minerals
            this.FindByName<Entry>("dri_calcium").Text = calcium[saveNum];
            this.FindByName<Entry>("dri_iron").Text = iron[saveNum];
            this.FindByName<Entry>("dri_magnesium").Text = magnesium[saveNum];
            this.FindByName<Entry>("dri_phosphorus").Text = phosphorus[saveNum];
            this.FindByName<Entry>("dri_potassium").Text = potassium[saveNum];
            this.FindByName<Entry>("dri_sodium").Text = sodium[saveNum];
            this.FindByName<Entry>("dri_zinc").Text = zinc[saveNum];
            this.FindByName<Entry>("dri_copper").Text = copper[saveNum];
            this.FindByName<Entry>("dri_manganese").Text = manganese[saveNum];
            this.FindByName<Entry>("dri_selenium").Text = selenium[saveNum];

            //save for the nutrient graphs
            saveInfo();

        }

        //calculate the index
        public void calculateSaveNum()
        {
            var db = DataAccessor.getDataAccessor();
            //4 values needed to calculate DRI
            String age = db.getSettingsItem("age");

            if (age != null && age != "")
            {
                ageNum = int.Parse(age);

            }

            String genderString = db.getSettingsItem("gender");
            Console.WriteLine("GS = " + genderString);
            int.TryParse(genderString, out gender);
            //gender = int.Parse(genderString);

            String pregnantString = db.getSettingsItem("pregnant");
            Console.WriteLine("PS = " + pregnantString);
            int.TryParse(pregnantString, out pregnant);
            //pregnant = int.Parse(pregnantString);

            String lactatingString = db.getSettingsItem("lactating");
            Console.WriteLine("LS = " + lactatingString);
            int.TryParse(lactatingString, out lactating);
            //lactating = int.Parse(lactatingString);


            //calculations for infants
            if (ageNum < 1)
            {
                saveNum = 0;

            }

            //Calculations for Children
            else if (ageNum >= 1 && ageNum <= 3)
            {
                saveNum = 1;
            }
            //calculations for older children
            else if (ageNum >= 4 && ageNum <= 8)
            {
                saveNum = 2;

            }
            //calculations for adults
            else
            {
                //Males
                if (gender == 1)
                {
                    //9-13
                    if (ageNum >= 9 && ageNum <= 13)
                    {
                        saveNum = 3;

                    }
                    //14-18
                    else if (ageNum >= 14 && ageNum <= 18)
                    {
                        saveNum = 4;
                    }
                    //19-30
                    else if (ageNum >= 19 && ageNum <= 30)
                    {
                        saveNum = 5;

                    }
                    //31-50
                    else if (ageNum >= 31 && ageNum <= 50)
                    {
                        saveNum = 6;

                    }
                    //51-70
                    else if (ageNum >= 51 && ageNum <= 70)
                    {
                        saveNum = 7;

                    }
                    //70+
                    else if (ageNum > 70)
                    {
                        saveNum = 8;

                    }
                }
                //female
                else if (gender == 0)
                {
                    //not pregnant or lactating
                    if (pregnant == 0 && lactating == 0)
                    {
                        //9-13
                        if (ageNum >= 9 && ageNum <= 13)
                        {
                            saveNum = 9;

                        }
                        //14-18
                        else if (ageNum >= 14 && ageNum <= 18)
                        {
                            saveNum = 10;

                        }
                        //19-30
                        else if (ageNum >= 19 && ageNum <= 30)
                        {
                            saveNum = 11;

                        }

                        //31-50
                        else if (ageNum >= 31 && ageNum <= 50)
                        {
                            saveNum = 12;

                        }
                        //51-70
                        else if (ageNum >= 51 && ageNum <= 70)
                        {
                            saveNum = 13;

                        }
                        //70+
                        else if (ageNum > 70)
                        {
                            saveNum = 14;

                        }
                    }
                    //pregnant women
                    else if (pregnant == 1 && lactating == 0)
                    {
                        //14-18
                        if (ageNum >= 14 && ageNum <= 18)
                        {
                            saveNum = 15;

                        }
                        //19-30
                        else if (ageNum >= 19 && ageNum <= 30)
                        {
                            saveNum = 16;

                        }
                        //31-50
                        else if (ageNum >= 31 && ageNum <= 50)
                        {

                            saveNum = 17;

                        }
                    }
                    //lactating women
                    else if (pregnant == 0 && lactating == 1)
                    {
                        //14-18
                        if (ageNum >= 14 && ageNum <= 18)
                        {
                            saveNum = 18;

                        }
                        //19-30
                        else if (ageNum >= 19 && ageNum <= 30)
                        {
                            saveNum = 19;

                        }
                        //31-50
                        else if (ageNum >= 31 && ageNum <= 50)
                        {
                            saveNum = 20;

                        }
                    }

                }

            }
        }

        private string calculateCalories()
        {
            var db = DataAccessor.getDataAccessor();

            //Calculating Calories using Mifflin-St. Jeor equation
            String ageString = db.getSettingsItem("age");
            var age = int.Parse(ageString);
            String genderString = db.getSettingsItem("gender");
            gender = int.Parse(genderString); //male == 1, female == 0
            String weightString = db.getSettingsItem("weight");
            var weight_kg = double.Parse(weightString) * 0.453592;
            String feet = db.getSettingsItem("feet");
            String inches = db.getSettingsItem("inches");
            var height_cm = (double.Parse(feet) * 12 + double.Parse(inches)) * 2.54;
            String activityLevelString = db.getSettingsItem("activity_level");
            var activityLevel = double.Parse(activityLevelString);
            var calories = 0;

            if (gender == 1)
            {
                calories = (int)((10 * weight_kg + 6.25 * height_cm - 5 * age + 5) * activityLevel);
            }
            else
            {
                calories = (int)((10 * weight_kg + 6.25 * height_cm - 5 * age - 161) * activityLevel);
            }
            return calories.ToString();
        }

        private string calculateSugar(string calories)
        {
            var sugar = (int)(0.1 * double.Parse(calories)) / 4;
            return sugar.ToString();
        }

        public void saveInfo()
        {
            Console.WriteLine("saved");
            var db = DataAccessor.getDataAccessor();

            //macronutrients
            foreach (var macroNutrient in this.macronutrients)
            {
                db.saveDRIValue(macroNutrient, this.FindByName<Entry>(macroNutrient).Text);
            }

            //vitamins
            foreach (var vitamin in this.vitamins)
            {
                db.saveDRIValue(vitamin, this.FindByName<Entry>(vitamin).Text);
            }

            //minerals
            foreach (var mineral in this.minerals)
            {
                db.saveDRIValue(mineral, this.FindByName<Entry>(mineral).Text);
            }
        }

        //save without loading the page, need for the visualizer
        /// <summary>
        /// Sugar and Calories Calculated
        /// dri values and threshold values saved here 
        /// Can edit the threshold values per nutrient here. 
        /// </summary>
		public void saveNoLoad()
        {
            var db = DataAccessor.getDataAccessor();
            calculateSaveNum();

            var calories = calculateCalories();
            var sugar = calculateSugar(calories);

            //default threshold percentages of dri
            var lowDefault = 0.25;
            var highDefault = 1.25;

            //macronutrients
            db.saveDRIValue("dri_omega6/3 ratio", "4");
            db.saveDRIThresholds("dri_omega6/3 ratio", "4", "10");

            db.saveDRIValue("dri_calories", calories.ToString());
            db.saveDRIThresholds("dri_calories",
                                 (Convert.ToDouble(calories) * lowDefault).ToString(),
                                 (Convert.ToDouble(calories) * highDefault).ToString());

            db.saveDRIValue("dri_totalCarbs", totalCarbs[saveNum]);
            db.saveDRIThresholds("dri_totalCarbs",
                                 (Convert.ToDouble(totalCarbs[saveNum]) * lowDefault).ToString(),
                                (Convert.ToDouble(totalCarbs[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_sugar", sugar.ToString());
            db.saveDRIThresholds("dri_sugar",
                                 (Convert.ToDouble(sugar) * lowDefault).ToString(),
                                 (Convert.ToDouble(sugar) * highDefault).ToString());

            db.saveDRIValue("dri_dietaryFiber", dietaryFiber[saveNum]);
            db.saveDRIThresholds("dri_dietaryFiber",
                                 (Convert.ToDouble(dietaryFiber[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(dietaryFiber[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_netCarbs", netCarbs[saveNum]);
            db.saveDRIThresholds("dri_netCarbs",
                                 (Convert.ToDouble(netCarbs[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(netCarbs[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_protein", protein[saveNum]);
            db.saveDRIThresholds("dri_protein",
                                 (Convert.ToDouble(protein[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(protein[saveNum]) * highDefault).ToString());
            //vitamins
            db.saveDRIValue("dri_vitaminA", vitaminA[saveNum]);
            db.saveDRIThresholds("dri_vitaminA",
                                 (Convert.ToDouble(vitaminA[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(vitaminA[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_vitaminC", vitaminC[saveNum]);
            db.saveDRIThresholds("dri_vitaminC",
                                 (Convert.ToDouble(vitaminC[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(vitaminC[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_vitaminD", vitaminD[saveNum]);
            db.saveDRIThresholds("dri_vitaminD",
                                 (Convert.ToDouble(vitaminD[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(vitaminD[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_vitaminE", vitaminE[saveNum]);
            db.saveDRIThresholds("dri_vitaminE",
                                 (Convert.ToDouble(vitaminE[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(vitaminE[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_vitaminK", vitaminK[saveNum]);
            db.saveDRIThresholds("dri_vitaminK",
                                 (Convert.ToDouble(vitaminK[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(vitaminK[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_thiamin", thiamin[saveNum]);
            db.saveDRIThresholds("dri_thiamin",
                                 (Convert.ToDouble(thiamin[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(thiamin[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_riboflavin", riboflavin[saveNum]);
            db.saveDRIThresholds("dri_riboflavin",
                                 (Convert.ToDouble(riboflavin[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(riboflavin[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_niacin", niacin[saveNum]);
            db.saveDRIThresholds("dri_niacin",
                                 (Convert.ToDouble(niacin[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(niacin[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_vitaminB6", vitaminB6[saveNum]);
            db.saveDRIThresholds("dri_vitaminB6",
                                 (Convert.ToDouble(vitaminB6[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(vitaminB6[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_folate", folate[saveNum]);
            db.saveDRIThresholds("dri_folate",
                                 (Convert.ToDouble(folate[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(folate[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_vitaminB12", vitaminB12[saveNum]);
            db.saveDRIThresholds("dri_vitaminB12",
                                 (Convert.ToDouble(vitaminB12[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(vitaminB12[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_pantothenicAcid", pantothenicAcid[saveNum]);
            db.saveDRIThresholds("dri_pantothenicAcid",
                                 (Convert.ToDouble(pantothenicAcid[saveNum]) * lowDefault).ToString(),
                                (Convert.ToDouble(pantothenicAcid[saveNum]) * highDefault).ToString());

            //minerals
            db.saveDRIValue("dri_calcium", calcium[saveNum]);
            db.saveDRIThresholds("dri_calcium",
                                 (Convert.ToDouble(calcium[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(calcium[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_iron", iron[saveNum]);
            db.saveDRIThresholds("dri_iron",
                                 (Convert.ToDouble(iron[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(iron[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_magnesium", magnesium[saveNum]);
            db.saveDRIThresholds("dri_magnesium",
                                 (Convert.ToDouble(magnesium[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(magnesium[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_phosphorus", phosphorus[saveNum]);
            db.saveDRIThresholds("dri_phosphorus",
                                 (Convert.ToDouble(phosphorus[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(phosphorus[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_potassium", potassium[saveNum]);
            db.saveDRIThresholds("dri_potassium",
                                 (Convert.ToDouble(potassium[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(potassium[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_sodium", sodium[saveNum]);
            db.saveDRIThresholds("dri_sodium",
                                 (Convert.ToDouble(sodium[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(sodium[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_zinc", zinc[saveNum]);
            db.saveDRIThresholds("dri_zinc",
                                 (Convert.ToDouble(zinc[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(zinc[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_copper", copper[saveNum]);
            db.saveDRIThresholds("dri_copper",
                                 (Convert.ToDouble(copper[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(copper[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_manganese", manganese[saveNum]);
            db.saveDRIThresholds("dri_manganese",
                                 (Convert.ToDouble(manganese[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(manganese[saveNum]) * highDefault).ToString());

            db.saveDRIValue("dri_selenium", selenium[saveNum]);
            db.saveDRIThresholds("dri_selenium",
                                 (Convert.ToDouble(selenium[saveNum]) * lowDefault).ToString(),
                                 (Convert.ToDouble(selenium[saveNum]) * highDefault).ToString());
        }

        async void GetHelpInfo(object sender, EventArgs e)
        {
            await DisplayAlert("DRI Information", "Dietary Reference Intakes refers to a set of values that assess your nutrition intake. These values are the recommended minimal values. You may set your own target values based on your needs and goals.", "OK");
        }

        async void CustomizeDRIs(object sender, EventArgs e)
        {
            var selection = await DisplayAlert("Customize", "Are you sure you want to customize your recommended Dietary Reference Intakes?", "Yes", "No");
            if (selection == true)
            {
                var db = DataAccessor.getDataAccessor();
                //set as customized
                db.saveSettingsItem("custom_dri", "true");
                //save all of the custom values
                saveInfo();
            }
        }

        async void ResetDRIs(object sender, EventArgs e)
        {
            var selection = await DisplayAlert("Reset", "Are you sure you want to reset your Dietary Reference Intakes to the minimum recommended values?", "Yes", "No");
            if (selection == true)
            {
                //set as not customized
                var db = DataAccessor.getDataAccessor();
                db.saveSettingsItem("custom_dri", "false");
                //populate fields
                setValues();
            }
        }
    }
}