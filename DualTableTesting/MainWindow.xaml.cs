using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectTaskDLL;

namespace DualTableTesting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProjectTaskClass TheProjectTasksClass = new ProjectTaskClass();

        ProductivityDataSet aProductivityDataSet;
        ProductivityDataSetTableAdapters.FindProductivityFootageByAssignedProjectIDTableAdapter aFindProductivityFootageTableAdapter;
        ProductivityDataSetTableAdapters.FindProductivityHoursByAssignedProjectIDTableAdapter aFindProductivityHoursTableAdapter;
        ProductivityDataSet TheProductivityDataSet;
        CompleteProductivityDataSet TheCompleteProductivityDataSet = new CompleteProductivityDataSet();
        FindSpecificProjectWorkTaskDataSet TheFindSpecificProjectWorkTaskDataSet = new FindSpecificProjectWorkTaskDataSet();

        int gintProductivityCounter;
        int gintProductivityUpperLimit;
       
        public MainWindow()
        {
            InitializeComponent();
        }
       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadControls("TESTING");
        }
        private ProductivityDataSet GetProductivity(string strProjectID)
        {
            aProductivityDataSet = new ProductivityDataSet();
            aFindProductivityFootageTableAdapter = new ProductivityDataSetTableAdapters.FindProductivityFootageByAssignedProjectIDTableAdapter();
            aFindProductivityHoursTableAdapter = new ProductivityDataSetTableAdapters.FindProductivityHoursByAssignedProjectIDTableAdapter();
            aFindProductivityFootageTableAdapter.Fill(aProductivityDataSet.FindProductivityFootageByAssignedProjectID, strProjectID);
            aFindProductivityHoursTableAdapter.Fill(aProductivityDataSet.FindProductivityHoursByAssignedProjectID, strProjectID);

            return aProductivityDataSet;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadControls(txtProjectID.Text);
        }
        private void LoadControls(string strAssignedProjectID)
        {
            int intFirstCounter;
            int intFirstNumberOfRecords;
            int intTaskID;
            int intProjectID;
            int intProductivityCounter;
            bool blnItemFound = false;
            DateTime datTransctionDate;
            int intSecondCounter;
            int intSecondNumberOfRecords;

            TheCompleteProductivityDataSet.productivity.Rows.Clear();

            TheProductivityDataSet = GetProductivity(strAssignedProjectID);

            intFirstNumberOfRecords = TheProductivityDataSet.FindProductivityHoursByAssignedProjectID.Rows.Count - 1;
            gintProductivityCounter = 0;
            gintProductivityUpperLimit = 0;

            for(intFirstCounter = 0; intFirstCounter <= intFirstNumberOfRecords; intFirstCounter++)
            {
                intTaskID = TheProductivityDataSet.FindProductivityHoursByAssignedProjectID[intFirstCounter].TaskID;
                intProjectID = TheProductivityDataSet.FindProductivityHoursByAssignedProjectID[intFirstCounter].ProjectID;
                blnItemFound = false;
                datTransctionDate = TheProductivityDataSet.FindProductivityHoursByAssignedProjectID[intFirstCounter].TransactionDate;
               
                if(gintProductivityCounter > 0)
                {
                    for(intProductivityCounter = 0; intProductivityCounter <= gintProductivityUpperLimit; intProductivityCounter++)
                    {
                        if(intTaskID == TheCompleteProductivityDataSet.productivity[intProductivityCounter].WorkTaskID)
                        {
                            if (datTransctionDate == TheCompleteProductivityDataSet.productivity[intProductivityCounter].TransactionDate)
                            {
                                TheCompleteProductivityDataSet.productivity[intProductivityCounter].TotalHours += TheProductivityDataSet.FindProductivityHoursByAssignedProjectID[intFirstCounter].TotalHours;
                                blnItemFound = true;
                            }
                        }
                    }
                }
                
                if(blnItemFound == false)
                {
                    CompleteProductivityDataSet.productivityRow NewProductionRow = TheCompleteProductivityDataSet.productivity.NewproductivityRow();

                    NewProductionRow.FootagePieces = 0;
                    NewProductionRow.ProjectID = intProjectID;
                    NewProductionRow.TotalHours = TheProductivityDataSet.FindProductivityHoursByAssignedProjectID[intFirstCounter].TotalHours;
                    NewProductionRow.TransactionDate = datTransctionDate;
                    NewProductionRow.WorkTask = TheProductivityDataSet.FindProductivityHoursByAssignedProjectID[intFirstCounter].WorkTask;
                    NewProductionRow.WorkTaskID = intTaskID;

                    TheCompleteProductivityDataSet.productivity.Rows.Add(NewProductionRow);
                    gintProductivityUpperLimit = gintProductivityCounter;
                    gintProductivityCounter++;
                }
            }

            intFirstNumberOfRecords = TheCompleteProductivityDataSet.productivity.Rows.Count - 1;

            for(intFirstCounter = 0; intFirstCounter <= intFirstNumberOfRecords; intFirstCounter++)
            {
                intTaskID = TheCompleteProductivityDataSet.productivity[intFirstCounter].WorkTaskID;
                intProjectID = TheCompleteProductivityDataSet.productivity[intFirstCounter].ProjectID;
                datTransctionDate = TheCompleteProductivityDataSet.productivity[intFirstCounter].TransactionDate;

                TheFindSpecificProjectWorkTaskDataSet = TheProjectTasksClass.FindSpecificProjectWorkTask(intProjectID, intTaskID);

                intSecondNumberOfRecords = TheFindSpecificProjectWorkTaskDataSet.FindSpecificProjectWorkTask.Rows.Count - 1;

                for(intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                {
                    if (datTransctionDate == TheFindSpecificProjectWorkTaskDataSet.FindSpecificProjectWorkTask[intSecondCounter].TransactionDate)
                    {
                        if(TheCompleteProductivityDataSet.productivity[intFirstCounter].FootagePieces == 0)
                        {
                            TheCompleteProductivityDataSet.productivity[intFirstCounter].FootagePieces = TheFindSpecificProjectWorkTaskDataSet.FindSpecificProjectWorkTask[intSecondCounter].FootagePieces;
                        }
                    }
                }
            }

            dgrHours.ItemsSource = TheCompleteProductivityDataSet.productivity;
        }
    }
}
