using NuMo_Tabbed.DatabaseItems;
using NuMo_Tabbed.ItemViews;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static NuMo_Tabbed.NumoNameSearch;

namespace NuMo_Tabbed
{
    //Implements Singleton design pattern: This class is the focus point of all database queries
    class DataAccessor
    {
        private static DataAccessor accessor;
        SQLiteConnection dbConn;

        private DataAccessor()
        {
            dbConn = DependencyService.Get<ISQLite>().GetConnection();

        }

        public static DataAccessor getDataAccessor()
        {
            if (accessor == null)
            {
                accessor = new NuMo_Tabbed.DataAccessor();
            }
            return accessor;
        }

        //For use in addItem pages.
        public List<NumoNameSearch> searchName(String name)
        {

            if (name == null || name.Length <= 0 || name.Trim().Length == 0)
                return new List<NumoNameSearch>();
            //ignore single quotes
            name = name.Replace("’", "");//iPhone specific
            name = name.Replace("'", "");//Android Specific
            name = name.ToUpper();
            String where = "WHERE ";
            String whereOR = "WHERE ";
            var names = name.Split(' ');

            //build where clause.
            foreach (var word in names)
            {
                if (word.Length > 0)
                {
                    var temp = word;
                    //Removes plural form, but still catches correct form using wild cards
                    if (word[word.Length - 1] == 'S')
                    {
                        temp = word.Substring(0, word.Length - 1);
                    }

                    where += String.Format("UPPER(Long_Desc) LIKE '%{0}%' AND ", temp);
                    whereOR += String.Format("UPPER(Long_Desc) LIKE '%{0}%' OR ", temp); //Allows for misspelling of a word in a phrase
                }
            }
            where = where.Remove(where.Length - 4);//remove last 4 as we don't want the final 'AND '
            whereOR = whereOR.Remove(whereOR.Length - 3);//remove last 3 as we don't want the final 'OR '
            var query = String.Format("SELECT NDB_No as food_no, Long_Desc as name FROM FOOD_DES {0}order by Times_Searched DESC", where);
            var queryOR = String.Format("SELECT NDB_No as food_no, Long_Desc as name FROM FOOD_DES {0}order by Times_Searched DESC", whereOR);

            var resultList = dbConn.Query<NumoNameSearch>(query);
            var resultsOR = dbConn.Query<NumoNameSearch>(queryOR);
            foreach (var item in resultsOR)
                resultList.Add(item);
            return resultList;
        }

        //Retrieve custom quantifiers from database and add to searchItem
        public void addCustomQuantifiers(NumoNameSearch item)
        {
            string data_num = item.food_no.ToString();
            if (data_num.Length == 4)
                data_num = "0" + data_num;

            var convertItems = getCustomQuantifiers(item.food_no);
            item.convertions.AddRange(convertItems);
        }

        //Retrieves custom quantifiers from the database for a food_no
        public List<ConvertItem> getCustomQuantifiers(int food_no)
        {
            string data_num = food_no.ToString();
            if (data_num.Length == 4)
                data_num = "0" + data_num;
            var quantifier1 = dbConn.Query<ConvertItem>(String.Format("SELECT GmWt_1 as gramsMultiplier, GmWt_Desc1 as name FROM ABBREV WHERE NDB_No = '{0}'", data_num));
            var quantifier2 = dbConn.Query<ConvertItem>(String.Format("SELECT GmWt_2 as gramsMultiplier, GmWt_Desc2 as name FROM ABBREV WHERE NDB_No = '{0}'", data_num));

            foreach (var item2 in quantifier2)
            {
                if (item2.name != null)
                {
                    quantifier1.AddRange(quantifier2);
                    break;
                }
            }

            return quantifier1;
        }

        //Retrives macronutrients from the database for a food_no
        //NOT WORKING
        public List<ConvertItem> getFoodInfo(int food_no)
        {
            string data_num = food_no.ToString();
            if (data_num.Length == 4)
                data_num = "0" + data_num;
            var foodInfo = dbConn.Query<ConvertItem>(String.Format("SELECT Carbohydrt_(g) as carbs, Sugar_Tot_(g) as sugarTotal FROM ABBREV WHERE NDB_No = '{0}'", data_num));
            return foodInfo;
        }


        //Retrieves one clean list of all the nutrients related to a list of historyItems
        public List<Nutrient> getNutrientsFromHistoryList(List<FoodHistoryItem> historyItems)
        {
            var list = new List<FoodItem>();
            foreach (var historyItem in historyItems)
            {
                var foodItem = getFoodItem(historyItem.food_no, UnitConverter.getMultiplier(historyItem.Quantifier, historyItem.food_no) * historyItem.Quantity);
                list.Add(foodItem);
            }
            var combinedFoodItem = new FoodItem(list);
            combinedFoodItem.stripExtraOmegs();
            return combinedFoodItem.nutrients;
        }

        //Retrive a foodItem with nutrients calculated for the input quantityInGrams
        public FoodItem getFoodItem(int food_no, double quantityInGrams)
        {
            string data_num = food_no.ToString();
            if (data_num.Length == 4)
                data_num = "0" + data_num;
            var values = dbConn.Query<NutData>(String.Format("Select NDB_No as food_no, Nutr_No as nutr_no, Nutr_Val as nutr_value FROM NUT_DATA WHERE NDB_No = '{0}'", data_num));

            FoodItem foodItem = new NuMo_Tabbed.FoodItem(values, quantityInGrams);
            return foodItem;
        }

        //Used to save and read Settings and User Info from the database
        public void saveSettingsItem(String settingName, String settingValue)
        {
            var values = dbConn.Query<MyDayReminderItem>(String.Format("SELECT Setting_Name from SETTINGS WHERE Setting_Name = '{0}'", settingName));
            if (values.Any())
            {
                dbConn.Execute(String.Format("UPDATE SETTINGS set Setting_Val = '{0}' WHERE Setting_Name = '{1}'", settingValue, settingName));
            }
            else
            {
                dbConn.Execute(String.Format("INSERT INTO SETTINGS (Setting_Name, Setting_Val) VALUES ('{0}', '{1}')", settingName, settingValue));
            }
        }

        public string getSettingsItem(String settingName)
        {
            //uncomment the following two lines to clear the user profile data
            //dbConn.Query<MyDayReminderItem>(String.Format("Delete from SETTINGS")); //use this to delete profile keep return ""; also
            //dbConn.Query<MyDayReminderItem>(String.Format("Delete from DRI_VALUES")); //use this to delete DRI_Values keep return ""; also
              var values = dbConn.Query<MyDayReminderItem>(String.Format("SELECT Setting_Val as imageString from SETTINGS WHERE Setting_Name = '{0}'", settingName));
             if (values.Any())
                 return values.First().imageString;
             else
                 return "";
                 
        }

        public void saveDRIValue(String DRIName, String DRIValue)
        {
            var values = dbConn.Query<MyDayReminderItem>(String.Format("SELECT DRI_Name from DRI_VALUES WHERE DRI_Name = '{0}'", DRIName));
            if (values.Any())
            {
                dbConn.Execute(String.Format("UPDATE DRI_VALUES set DRI_Val = '{0}' WHERE DRI_Name = '{1}'", DRIValue, DRIName));
            }
            else
            {
                dbConn.Execute(String.Format("INSERT INTO DRI_VALUES (DRI_Name, DRI_Val) VALUES ('{0}', '{1}')", DRIName, DRIValue));
            }
        }

        public string getDRIValue(String DRIName)
        {
            var args = new List<object>();
            args.Add(DRIName);
            var values = dbConn.Query<MyDayReminderItem>(String.Format("SELECT DRI_Val as imageString from DRI_VALUES WHERE DRI_Name = '{0}'", args.ToArray()));
            if (values.Any())
                return values.First().imageString;
            else
                return "";
        }

        //save threshold values for dri thresholds
        public void saveDRIThresholds(String DRIName, String lowThresh, String highThresh)
        {
            var values = dbConn.Query<DRIThresholds>(String.Format("SELECT Low_Thresh as lowThresh from DRI_VALUES WHERE DRI_Name = '{0}'", DRIName));
            if (values.Any())
            {
                dbConn.Execute(String.Format("UPDATE DRI_VALUES set Low_Thresh = '{0}', High_Thresh ='{1}' WHERE DRI_Name = '{2}'", lowThresh, highThresh, DRIName));
            }
            else
            {
                dbConn.Execute(String.Format("INSERT INTO DRI_VALUES (DRI_Name, Low_Thresh, High_Thresh) VALUES ('{0}', '{1}', '{2}')", DRIName, lowThresh, highThresh));
            }
        }

        //retrieve the thresholds of whichever nutrient
        public List<DRIThresholds> getDRIThresholds(String DRIName)
        {
            var values = dbConn.Query<DRIThresholds>(String.Format("SELECT Low_Thresh as lowThresh, High_Thresh as highThresh from DRI_VALUES WHERE DRI_Name = '{0}'", DRIName));

            return values;
        }

        //Inserts a new reminder for a specific date. Reminders are held in the database as a base64 string.
        public void insertReminder(MyDayReminderItem item)
        {
            if (item.imageString == null)
            {
                StreamImageSource streamImageSource = (StreamImageSource)item.ReminderImage.Source;
                System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
                Task<Stream> task = streamImageSource.Stream(cancellationToken);
                Stream stream = task.Result;
                var memStream = new MemoryStream();
                stream.CopyTo(memStream);
                item.imageString = Convert.ToBase64String(memStream.ToArray());
            }
            dbConn.Execute(String.Format("INSERT INTO FoodHistory (Date, Image, Food_Id, Quantity, Quantifier) VALUES ('{0}', '{1}', 0, 0, '')", item.Date, item.imageString));
            dbConn.Commit();
        }

        //retrieves reminder from database and converts it back to an image from base64.
        public List<MyDayReminderItem> getReminders(String date)
        {
            var baseList = dbConn.Query<MyDayReminderItem>(String.Format("SELECT History_Id as id, date as Date, Image as imageString FROM FoodHistory WHERE date='{0}' AND Image NOT NULL", date));
            //convert the image string to actual image in the MyDayReminderItem TODO
            foreach (var item in baseList)
            {
                var imageString = item.imageString;
                var byteArray = Convert.FromBase64String(imageString);
                Image image = new Image();
                image.Source = ImageSource.FromStream(() => new MemoryStream(byteArray));
                item.ReminderImage = image;
            }
            return baseList;

        }


        public String getNameFromID(int food_no)
        {
            var values = dbConn.Query<NumoNameSearch>(String.Format("SELECT Long_Desc as name FROM FOOD_DES where NDB_No = {0}", food_no));
            return values.First<NumoNameSearch>().name;
        }

        //quantity is the number of servings...
        public void createFoodItem(List<Nutrient> values, String name, double multiplier, String quantifier)
        {
            name.Replace("'", "''");
            dbConn.Execute(String.Format("INSERT INTO FOOD_DES (Long_Desc, Times_Searched) VALUES ('{0}', 10)", name));
            dbConn.Commit();
            var food_no_data = dbConn.Query<NumoNameSearch>(String.Format("SELECT NDB_No as food_no, Long_Desc as name FROM FOOD_DES where UPPER(Long_Desc) LIKE '{0}' order by Times_Searched DESC", name));
            var food_no = food_no_data.FirstOrDefault().food_no;
            String data_num = food_no.ToString();
            if (data_num.Length == 4)
                data_num = "0" + data_num;
            dbConn.Execute(String.Format("INSERT INTO ABBREV (NDB_No, GmWt_1, GmWt_Desc1) VALUES ('{0}', {1}, '{2}')", data_num, multiplier, quantifier));
            foreach (var value in values)
            {
                dbConn.Execute(String.Format("INSERT INTO NUT_DATA (NDB_No, Nutr_No, Nutr_Val) VALUES ('{0}', '{1}', {2})", food_no, value.dbNo, value.quantity * 100));//database stores based on 100 grams, we treat entries as 1 gram
            }
        }

        //Adds the foodHistory entry to the database
        public void addFoodHistory(FoodHistoryItem item)
        {
            dbConn.Execute(String.Format("INSERT INTO FoodHistory (Date, Food_Id, Quantity, Quantifier) VALUES ('{0}', {1}, {2}, '{3}')", item.Date, item.food_no, item.Quantity, item.Quantifier));
        }



        //retrieves the foodhistory associated to a certain date
        public List<FoodHistoryItem> getFoodHistoryList(String date)
        {
            return dbConn.Query<FoodHistoryItem>(String.Format("SELECT History_Id, Date as Date, Food_Id as food_no, Quantity as Quantity, Quantifier as Quantifier FROM FoodHistory WHERE Date LIKE '%{0}%'", date));
        }

        //retrieves the foodhistory associated to a certain date and prepares it for display as a MyDayFoodItem
        public List<MyDayFoodItem> getFoodHistory(String date)
        {
            var historyList = dbConn.Query<FoodHistoryItem>(String.Format("SELECT History_Id, Date as Date, Food_Id as food_no, Quantity as Quantity, Quantifier as Quantifier FROM FoodHistory WHERE Date LIKE '%{0}%'", date));
            var resultList = new List<MyDayFoodItem>();
            foreach (var result in historyList)
            {
                if (result.food_no > 0)
                {
                    var newItem = new MyDayFoodItem();
                    newItem.id = result.History_Id;
                    newItem.DisplayName = getNameFromID(result.food_no);
                    newItem.quantity = "(" + result.Quantity.ToString() + ") " + result.Quantifier;
                    resultList.Add(newItem);
                }
            }
            return resultList;
        }


        //Update an entry in the foodhistory table.
        public void updateFoodHistory(FoodHistoryItem item, int id)
        {
            dbConn.Execute(String.Format("UPDATE FoodHistory SET Quantity = {0}, Quantifier = '{1}', Food_Id = {2} WHERE History_id = {3}", item.Quantity, item.Quantifier, item.food_no, id));
        }

        //Retreives a foodhistory item by id
        public FoodHistoryItem getFoodHistoryItem(int foodHistoryId)
        {
            var historyList = dbConn.Query<FoodHistoryItem>(String.Format("SELECT History_Id, Date as Date, Food_Id as food_no, Quantity as Quantity, Quantifier as Quantifier FROM FoodHistory WHERE History_Id = {0}", foodHistoryId));
            return historyList.First<FoodHistoryItem>();
        }

        //Delete an entry in the foodHistory table
        public void deleteFoodHistoryItem(int id)
        {
            if (id >= 0)
            {
                dbConn.Execute(String.Format("DELETE FROM FoodHistory WHERE History_Id={0}", id));
            }
        }

        //increment the timessearched field so that this entry gets preference in future searches
        public void incrementTimesSearched(int food_no)
        {
            int changes = dbConn.Execute(String.Format("UPDATE FOOD_DES SET Times_Searched = Times_Searched + 1 WHERE NDB_No = {0}", food_no));
            dbConn.Commit();
        }

        /* method to get a list of each tracked nutrient as a readable name.
         * uses GetDRINames to get the initial list of DRI names        
         * uses DRIToName to convert the DRI names to readable names
         * returns a list of readable names        
         * used for visualize page writing
         */
        public List<String> GetNutNames()
        {
            var dris = GetDRINames();
            List<String> nameStrings = new List<string>();
            foreach (String DRIname in dris)
            {
                nameStrings.Add(DRIToName(DRIname));
            }
            return nameStrings;
        }

        /* method to get the entire list of DRI nutrient names used in the database
         * return the names of nutrients, starting with "dri_" and ending with the camel case name
         */
        public List<String> GetDRINames()
        {
            List<string> driNames = new List<string>();
            String[] nutrientNames = {"dri_protein", "dri_totalCarbs", "dri_calories", "dri_sugar", "dri_dietaryFiber", "dri_calcium", "dri_iron", "dri_magnesium", "dri_phosphorus",
                 "dri_potassium", "dri_sodium", "dri_zinc", "dri_copper", "dri_manganese", "dri_selenium", "dri_vitaminA",
                 "dri_vitaminE", "dri_vitaminC", "dri_thiamin", "dri_riboflavin", "dri_niacin", "dri_pantothenicAcid", "dri_vitaminB6",
                      "dri_folate", "dri_vitaminB12", "dri_vitaminK" };
            foreach (String str in nutrientNames)
            {
                driNames.Add(str);
            }

            return driNames;

        }

        /* method to convert the DRI nutrient name to a readable name
         * returns a string that is capitalized and has spaces between words
         * used to help the GetNutNames function in DataAccessor
         */
        private String DRIToName(String driString)
        {
            var nameString = driString.Split('_')[1];
            List<char> charList = new List<char>();

            foreach (char currentChar in nameString.ToArray())
            {
                if (Char.IsUpper(currentChar))
                {
                    charList.Add(' ');
                }

                charList.Add(currentChar);

            }

            charList[0] = Char.ToUpper(charList[0]);
            string returnString = new string(charList.ToArray());
            return returnString;
        }

        //save hydration log
        public void saveKeto(DateTime KetoDate, double KetoVal)
        {
            var values = dbConn.Query<MyDayReminderItem>(String.Format("SELECT Keto_Date from KETO_VALUES WHERE Keto_Date = '{0}'", KetoDate));
            if (values.Any())
            {
                dbConn.Execute(String.Format("UPDATE KETO_VALUES set Keto_Val = '{0}' WHERE Keto_Date = '{1}'", KetoVal, KetoDate));
            }
            else
            {
                dbConn.Execute(String.Format("INSERT INTO KETO_VALUES (Keto_Date, Keto_Val) VALUES ('{0}', '{1}')", KetoDate, KetoVal));
            }
        }

        public string getKeto(DateTime KetoDate)
        {
            var tableExist = dbConn.Query<MyDayReminderItem>(String.Format("SELECT name FROM sqlite_master WHERE type = '{0}' AND name = '{1}'", "table", "KETO_VALUES"));
            if (tableExist.Any())
            {
            }
            else
            {
                dbConn.Query<MyDayReminderItem>(String.Format("CREATE TABLE KETO_VALUES (Keto_Date datetime, Keto_Val double);"));
            }

            var values = dbConn.Query<MyDayReminderItem>(String.Format("SELECT Keto_Val as imageString from KETO_VALUES WHERE Keto_Date = '{0}'", KetoDate));
            if (values.Any())
                return values.First().imageString;
            else
                return "";
        }

        //retrieves the foodhistory associated to a certain date
        public List<MyDayReminderItem> getKetoHistory(DateTime date)
        {
            return dbConn.Query<MyDayReminderItem>(String.Format("SELECT Keto_Val as imageString from KETO_VALUES WHERE Keto_Date LIKE '%{0}%'", date));
        }

        //save hydration log
        public void saveHydralog(String HydraName, String HYDRValue)
        {
            var values = dbConn.Query<MyDayReminderItem>(String.Format("SELECT Hydr_Name from HYDR_VALUES WHERE Hydr_Name = '{0}'", HydraName));
            if (values.Any())
            {
                dbConn.Execute(String.Format("UPDATE HYDR_VALUES set HYDR_Val = '{0}' WHERE Hydr_Name = '{1}'", HYDRValue, HydraName));
            }
            else
            {
                dbConn.Execute(String.Format("INSERT INTO HYDR_VALUES (Hydr_Name, HYDR_Val) VALUES ('{0}', '{1}')", HydraName, HYDRValue));
            }
        }

        public string getHydralog(String HydraName)
        {
            var tableExist = dbConn.Query<MyDayReminderItem>(String.Format("SELECT name FROM sqlite_master WHERE type = '{0}' AND name = '{1}'", "table", "HYDR_VALUES"));
            if (tableExist.Any())
            {
            }
            else
            {
                dbConn.Query<MyDayReminderItem>(String.Format("CREATE TABLE HYDR_VALUES (Hydr_Name varchar(20), Hydr_Val varchar(20));"));
            }

            var values = dbConn.Query<MyDayReminderItem>(String.Format("SELECT Hydr_Val as imageString from HYDR_VALUES WHERE Hydr_Name = '{0}'", HydraName));
            if (values.Any())
                return values.First().imageString;
            else
                return "";
        }

        //save picture urls for days
        public void savePicReminder(String picDate, String picPath, String picNum)
        {
            var values = dbConn.Query<MyDayReminderItem>(String.Format("SELECT Pic_Name from PIC_VALUES WHERE Pic_Name = '{0}' AND Pic_Num = '{1}'", picDate, picNum));
            if (values.Any())
            {
                dbConn.Execute(String.Format("UPDATE PIC_VALUES set Pic_Val = '{0}' WHERE Pic_Name = '{1}' AND Pic_Num = '{2}'", picPath, picDate, picNum));
            }
            else
            {
                dbConn.Execute(String.Format("INSERT INTO PIC_VALUES (Pic_Name, Pic_Val, Pic_Num) VALUES ('{0}', '{1}','{2}')", picDate, picPath, picNum));
            }
        }
        //get the urls for stored pictures by date and number
        public string getPicReminder(String picDate, String picNum)
        {
            var tableExist = dbConn.Query<MyDayReminderItem>(String.Format("SELECT name FROM sqlite_master WHERE type = '{0}' AND name = '{1}'", "table", "PIC_VALUES"));
            if (tableExist.Any())
            {
            }
            else
            {
                dbConn.Query<MyDayReminderItem>(String.Format("CREATE TABLE PIC_VALUES (Pic_Name varchar(20), Pic_Val varchar(2000), Pic_Num varchar(2));"));
            }

            var values = dbConn.Query<MyDayReminderItem>(String.Format("SELECT Pic_Val as imageString from PIC_VALUES WHERE Pic_Name = '{0}' AND Pic_Num = '{1}'", picDate, picNum));
            if (values.Any())
                return values.First().imageString;
            else
                return "";
        }

        //Delete an entry in the picture table
        public void deletePicture(String picDate, String picNum)
        {
            var values = dbConn.Query<MyDayReminderItem>(String.Format("SELECT Pic_Name from PIC_VALUES WHERE Pic_Name = '{0}' AND Pic_Num = '{1}'", picDate, picNum));
            if (values.Any())
            {
                String filePath = getPicReminder(picDate, picNum);
                if (File.Exists(filePath))
                {
                    //File.Delete(filePath);    //doesn't seem to delete fully, phone thinks image is still there when its not, which can cause weird crashes
                    Console.WriteLine("File Deleted");
                }
                dbConn.Execute(String.Format("DELETE FROM PIC_VALUES WHERE Pic_Name = '{0}' AND Pic_Num = '{1}'", picDate, picNum));


            }

        }


        //Determine if pics were taken this week
        public String getPreviousWeekPics(DateTime today)
        {
            String picDate = "";
            String foundPics = "";
            //only check today and last 6 days
            for (int i = 0; i < 6; i++)
            {
                picDate = today.AddDays((-1 * i)).ToString("MM/dd/yyyy");
                var values = dbConn.Query<MyDayReminderItem>(String.Format("SELECT Pic_Name from PIC_VALUES WHERE Pic_Name = '{0}'", picDate));
                if (values.Any())
                {
                    //concatenate the dates with pics into a string with new lines for display in alert
                    foundPics = "\t" + today.AddDays((-1 * i)).ToString("MM/dd/yyyy") + "\n" + foundPics;
                }
            }
            return foundPics;
        }

    }
}
