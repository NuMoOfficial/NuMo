using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NuMo_Tabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NutrFacts : ContentPage
    {

        public AddFoodPage aip;
        public NumoNameSearch selectedResult;

        public string Quantity
        {
            get
            {
                return quantity.Text;
            }
            set
            {
                quantity.Text = value;
            }
        }
        public string UnitPickerText
        {
            get
            {
                return UnitsPicker.Title;
            }
            set
            {
                UnitsPicker.Title = value;
            }
        }
        public string DescriptView
        {
            get
            {
                return descrView.Text;
            }
            set
            {
                descrView.Text = value;
            }
        }

        //empty constructor needed for visual studios preview
        public NutrFacts()
        {
            InitializeComponent();
        }

        public NutrFacts(AddFoodPage aip, NumoNameSearch search)
        {
            InitializeComponent();

            //save reference to food item
            selectedResult = search;

            //so the saveButtonClicked method can be used by the classes inheritting from AddItemPage
            this.aip = aip;

            //establish the save button on the top right
            ToolbarItem save = new ToolbarItem();
            save.Text = "Save";
            save.Clicked += saveButtonClicked;
            ToolbarItems.Add(save);

            //establish description view
            descrView.Text = search.ToString();
            descrView.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
            descrView.VerticalOptions = LayoutOptions.Start;
            descrView.HorizontalOptions = LayoutOptions.Start;

            //set default units in picker
            //add any custom picker units from selected item
            updateUnitPickerWithCustomOptions();

            //set default input values
            Quantity = "1";
            UnitsPicker.SelectedIndex = 0;
            UnitPickerText = UnitsPicker.SelectedItem.ToString();

            //so that it is not called incorrectly
            //get nutrient info on food item
            displayNutrInfo();
        }

        private void displayNutrInfo()
        {

            //check if there is a quantity
            if (Quantity != "")
            {
                //remove old info
                nutTable.Clear();

                var db = DataAccessor.getDataAccessor();
                //get selected food with nutrient info
                var foodItem = db.getFoodItem(selectedResult.food_no, UnitConverter.getMultiplier(getQuantifier(), selectedResult.food_no) * Convert.ToDouble(Quantity));

                //remove extra omega values, only want totals
                foodItem.stripExtraOmegs();

                //add preliminary table titles
                var colTitle = new StackLayout() { Orientation = StackOrientation.Horizontal };
                colTitle.Children.Add(new Label() //nutrients
                {
                    Text = "Nutrient",
                    WidthRequest = 150,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    //TextColor = (Color)App.Current.Resources["BtnBkgColor"],
                    FontAttributes = FontAttributes.Bold
                });
                colTitle.Children.Add(new Label()
                {
                    Text = "Amount",
                    //HorizontalOptions = LayoutOptions.StartAndExpand,
                    HorizontalOptions = LayoutOptions.EndAndExpand,

                    //TextColor = (Color)App.Current.Resources["BtnBkgColor"],
                    FontAttributes = FontAttributes.Bold
                });
                //////TODO show what these nutrient values are as percentages of the DRI
                //colTitle.Children.Add(new Label()
                //{
                //    Text = "DRI %",
                //    HorizontalOptions = LayoutOptions.End,
                //    TextColor = (Color)App.Current.Resources["BtnBkgColor"],
                //    FontAttributes = FontAttributes.Bold
                //});
                //add to table


                nutTable.Add(new ViewCell() { View = colTitle });
                //////////////////


                //add each nutrient into table
                foreach (var item in foodItem.nutrients)
                {

                    //create a new stack to put in table
                    var layout = new StackLayout() { Orientation = StackOrientation.Horizontal };
                    //stack contains nutrient name
                    layout.Children.Add(new Label()
                    {
                        Text = item.DisplayName,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        WidthRequest = 150,
                        //Style = App.Current.Resources["LabelStyle"] as Style
                    });
                    //stack contains nutrient quantity
                    layout.Children.Add(new Label()
                    {
                        Text = Convert.ToString(Math.Round(item.quantity, 4)),
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        //Style = App.Current.Resources["LabelStyle"] as Style
                    });

                    ///////////TODO add DRI %'s
                    //layout.Children.Add(new Label()
                    //{
                    //    Text = Convert.ToString(Math.Round(item.quantity, 4)),
                    //    HorizontalOptions = LayoutOptions.End,
                    //    Style = App.Current.Resources["LabelStyle"] as Style
                    //});

                    //add nutrient to xaml table
                    nutTable.Add(new ViewCell() { View = layout });
                }
            }
        }

        private void setBaseUnitPickerChoices()
        {
            foreach (var item in UnitConverter.standardUnits)
            {
                if (item != null)
                {
                    UnitsPicker.Items.Add(item);
                }
            }
            //add other base items here.
        }

        public void updateUnitPickerWithCustomOptions()
        {
            //reset entire picker
            UnitsPicker.Items.Clear();

            //to reset duplicate units appearing
            setBaseUnitPickerChoices();

            if (selectedResult != null)
            {
                var db = DataAccessor.getDataAccessor();
                db.addCustomQuantifiers(selectedResult);
                foreach (var converter in selectedResult.convertions)
                {
                    if (converter.name != null && !UnitsPicker.Items.Contains(converter.name) && converter.name != "")
                    {
                        UnitsPicker.Items.Insert(0, converter.name);
                    }
                }
            }
        }

        //return item from the picker if there is one selected
        public String getQuantifier()
        {
            if (UnitsPicker.SelectedIndex >= 0)
                return UnitsPicker.Items[UnitsPicker.SelectedIndex];
            else if (UnitsPicker.Title != null)
            {
                return UnitsPicker.Title;
            }
            else
            {
                return null;
            }
        }


        //clear all the fields when saved
        async void saveButtonClicked(object sender, EventArgs args)
        {
            //do not save unless input values are there
            if (Quantity == null || Quantity.Equals("") || Quantity.Equals("0"))
            {
                await DisplayAlert("Please input NUMBER OF value", "example: 1.5 or 3", "OK");

            }
            else if (UnitsPicker.SelectedIndex < 0 && UnitsPicker.Title.Equals("Units"))
            {
                await DisplayAlert("Please select UNITS value", "", "OK");
            }
            else
            {
                aip.saveButtonClicked(sender, args);
                await Navigation.PopAsync();
            }
        }

        //when picker changed, update nutrition info
        void Handle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            displayNutrInfo();
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            displayNutrInfo();
        }
    }
}

