using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MainApplication
{
    public partial class ItemForm : Form
    {
        //first we will create an identifier of the current transaction mode
        private enum TransactionMode { View, New, Edit };

        private TransactionMode _Mode;
        //lets create a getter and setter
        //so that we could easily control the objects
        private TransactionMode Mode
        {
            get { return _Mode; }
            set
            {
                _Mode = value;

                //now lets assign what will happen to the controls when the value changes

                //first enable/disable the databound controls such as textbox
                //all databound controls must be editable when not in view transaction mode
                txtCode.Enabled = value != TransactionMode.View;
                txtName.Enabled = value != TransactionMode.View;
                txtRemarks.Enabled = value != TransactionMode.View;
                chkActive.Enabled = value != TransactionMode.View;

                //now lets hide/show the buttons
                btnNew.Visible = value == TransactionMode.View;
                btnEdit.Visible = value == TransactionMode.View;
                btnSave.Visible = value != TransactionMode.View;
                btnDelete.Visible = value == TransactionMode.View;
                btnCancel.Visible = value != TransactionMode.View;

                //now lets clear the controls value depending on transaction mode
                if (value != TransactionMode.Edit)
                {
                    txtCode.Clear();
                    txtName.Clear();
                    txtRemarks.Clear();
                    chkActive.Checked = true;
                }
            }
        }

        //first we will define important variables
        private String ConnectionString;
        private SqlConnection Connection;
        private String CommandText;
        private SqlDataAdapter DataAdapter;
        private DataSet DataSet;
        private DataTable DataTable;

        private int SelectedId;

        public ItemForm()
        {
            InitializeComponent();

            //now lets set the values =========> FOR FELLOW DEVELOPERS KINDLY CHANGE THE CONNECTION STRING
            ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;User ID=sa;Initial Catalog=InventorySoftware;Data Source=.\SQLSERVER2016";
            Connection = new SqlConnection(ConnectionString);
            CommandText = "SELECT Id, Code, Name, Remarks, Active FROM tblItems";
            DataAdapter = new SqlDataAdapter(CommandText, Connection);
            DataSet = new DataSet();
            DataTable = new DataTable();

            //first we will create an instance of SqlCommandBuilder
            //this will automatically generate the Insert, Update, Delete statement
            SqlCommandBuilder CommandBuilder = new SqlCommandBuilder(DataAdapter);
        }

        private void ItemForm_Load(object sender, EventArgs e)
        {
            //upon loading of form the mode is in view
            Mode = TransactionMode.View;

            LoadData();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //upon clicking new the mode is in new hehe
            Mode = TransactionMode.New;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //same with others
            Mode = TransactionMode.Edit;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //if mode is new perform insert
            if (Mode == TransactionMode.New)
                InsertData();
            else if (Mode == TransactionMode.Edit)
                UpdateData();

            //reset the transaction mode
            Mode = TransactionMode.View;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Are you sure do you want to delete this record(s)?", "Application", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            if (CustomMessageBox.Show("Are you sure do you want to delete this record(s)?") == DialogResult.Yes)
                DeleteData();

            //set the transaction to view mode
            Mode = TransactionMode.View;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //if transaction is cancelled, transaction mode should return to view mode
            Mode = TransactionMode.View;
        }

        //now lets create a method that will load the data
        private void LoadData()
        {
            //clear the datatable first
            DataTable.Clear();

            //fill the schema so that it will also define automatically the primary key
            DataAdapter.FillSchema(DataSet, SchemaType.Source, "tblItems");

            //actual data fill
            DataAdapter.Fill(DataSet, "tblItems");

            DataTable = DataSet.Tables["tblItems"];

            dataGridView.DataSource = DataTable;
            dataGridView.AutoGenerateColumns = false;
        }

        //now lets create a method that will insert new record
        private void InsertData()
        {
            DataRow newRow = DataTable.NewRow();

            newRow["Code"] = txtCode.Text;
            newRow["Name"] = txtName.Text;
            newRow["Remarks"] = txtRemarks.Text;
            newRow["Active"] = chkActive.Checked;

            //lets add the new value on datatable
            DataTable.Rows.Add(newRow);

            //then lets call dataadapter.update
            DataAdapter.Update(DataSet, "tblItems");

            //finally reload the data
            LoadData();
        }

        //first we will set the textbox'es value based on selected record on grid
        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 0
                && Mode == TransactionMode.View)
            {
                SelectedId = int.Parse(dataGridView.SelectedRows[0].Cells[0].Value.ToString());

                txtCode.Text = dataGridView.SelectedRows[0].Cells[1].Value.ToString();
                txtName.Text = dataGridView.SelectedRows[0].Cells[2].Value.ToString();
                txtRemarks.Text = dataGridView.SelectedRows[0].Cells[3].Value.ToString();
                chkActive.Checked = bool.Parse(dataGridView.SelectedRows[0].Cells[4].Value.ToString());
            }

            //next we will disable the edit and delete button if no selected record on grid
            btnEdit.Enabled = Mode == TransactionMode.View && dataGridView.SelectedRows.Count != 0;
            btnDelete.Enabled = Mode == TransactionMode.View && dataGridView.SelectedRows.Count != 0;
        }

        //Now lets add the editing/updating
        private void UpdateData()
        {
            //find the row using the primary key "Id"

            //DataRow row = DataTable.Rows.Find(dataGridView.SelectedRows[0].Cells[0].Value);
            //will result to bug since the user may change the selection or remove the selection
            //upon editing of data, so SelectedId was supplied =========> BUG FIX

            DataRow row = DataTable.Rows.Find(SelectedId);

            //edit the datatable based on databound controls value
            row.BeginEdit();
            row["Code"] = txtCode.Text;
            row["Name"] = txtName.Text;
            row["Remarks"] = txtRemarks.Text;
            row["Active"] = chkActive.Checked;
            row.EndEdit();

            //update the data
            DataAdapter.Update(DataSet, "tblItems");

            //reload the data
            LoadData();
        }

        //first lets add the delete method
        private void DeleteData()
        {
            foreach (DataGridViewRow gridRow in dataGridView.SelectedRows)
            {
                //locate the row in datatable using the primary key "Id"
                DataRow row = DataTable.Rows.Find(gridRow.Cells[0].Value);

                row.Delete();
            }

            //actual database delete
            DataAdapter.Update(DataSet, "tblItems");

            //reload the data
            LoadData();
        }
    }
}
