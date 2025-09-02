using LockedIn.Data;
using LockedIn.Helpers;
using LockedIn.Managers;
using LockedIn.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;
using static LockedIn.Data.SessionData;
namespace LockedIn
{

    public partial class frmAdmin : Form
    {
        Prisoner thisPrisoner = null;
        Job selectedJob;

        string Ppassword = "";
        bool isAddingPrisoner = false;
        bool isDeletingPrisoner = false;
        bool isupdatingPrisoner = false;


        int skillID;
        int prsnrID;
        int selectedSkillID; // this is for editing and deleting tracking of skills
        string skillOperation = ""; // Track current skill operation: "EDIT", "DELETE", "ADD"

        string action = "";// this for maintain jobs to hold say if data needs to be edited, addded or deleted
        int selectedJobID = -1; // This will hold the selected JobID for Edit/Delete
        int CurrentEmployerID = 0;
        int JobID = 0;


        /*This flag checks if the comboboxes are currently busy populating (Because when they are, their indices change 
         * and they invoke the various indexchanged events and this causes several errors)*/
        bool isPopulating = false;
        public frmAdmin()
        {
            InitializeComponent();
            // Setting the two panels to edit and add skills to false 
            panel19.Visible = false;
            panel20.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 3;
            pnladJobDetails.Visible = false;
            pnlAdDate.Visible = false;
            pnlAdSkills.Visible = false;
            pnlAdFunctions.Visible = true;
        }



        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            isPopulating = true;
            //Popuating combo boxes.
            if (tcAdmin.SelectedIndex == 4)
            {
                cbxJobName.SelectedIndexChanged -= cbxJobName_SelectedIndexChanged;
                ComboBoxHelper.PopulateJobNames(cbxJobName);
                cbxJobName.SelectedIndexChanged += cbxJobName_SelectedIndexChanged;


                pnlJobDetails.Visible = true;
                pnlGuardDetails.Visible = false;
            }
            else if (tcAdmin.SelectedIndex == 6) //Maintain skills page
            {
                // Reset maintain skills panels when entering the page
                panel19.Visible = false;
                panel20.Visible = false;
                skillOperation = "";
                selectedSkillID = -1;

                ComboBoxHelper.PopulateSkillNames(cbxMSSkillName);
            }
            else if (tcAdmin.SelectedIndex == 7)
            {
                cmbPrisonerName.SelectedIndexChanged -= cmbPrisonerName_SelectedIndexChanged;
                ComboBoxHelper.PopulatePrisonerNumbers(cmbPrisonerName);
                cmbPrisonerName.SelectedIndexChanged += cmbPrisonerName_SelectedIndexChanged;
            }
            //Maintain Prisoner Page
            else if (tcAdmin.SelectedIndex == 8)
            {
                ComboBoxHelper.PopulatePrisonerNumbers(cmbMPPrisonerNo);
                pnlPInfo.Visible = false;
                pnlMPSkills.Visible = false;
                cmbMPPrisonerNo.Visible = false;
                pnlMPChoices.Visible = true;
                pnlModifyPPassword.Visible = false;
            }
            else if (tcAdmin.SelectedIndex == 2)
            {
                ComboBoxHelper.PopulateUncompletedJobNames(cbxCJJobName);
                calDateCompleted.MaxDate = DateTime.Today;
            }
            else if (tcAdmin.SelectedIndex == 5)
            {
                ComboBoxHelper.PopulateJobNames(cbxASJJobName);
            }
            else if (tcAdmin.SelectedIndex == 3)
            {

                pnladJobDetails.Visible = false;
                pnlAdDate.Visible = false;
                pnlAdSkills.Visible = false;
            }




            isPopulating = false;
        }


        private void cmbMPPrisonerNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isPopulating) return;

            if (isDeletingPrisoner)
            {
                pnlMPSkills.Enabled = false;
                pnlPInfo.Enabled = false;
                cmbMPPrisonerNo.Enabled = true;
            }


            if (cmbMPPrisonerNo.SelectedIndex != -1)
            {
                prsnrID = ComboBoxHelper.GetSelectedID(cmbMPPrisonerNo);

                isPopulating = true;
                ListboxHelper.PopulateAvailablePSkills(lstMPAllSkills, prsnrID);
                ListboxHelper.PopulatePrisonerSkills(lstMPCurrentSkills, prsnrID);
                isPopulating = false;

                thisPrisoner = PrisonerManager.LoadPrisoner(prsnrID);
                Ppassword = thisPrisoner.Password;
                txtPNo.Text = thisPrisoner.PrisonerNum;
                txtFirstName.Text = thisPrisoner.FirstName;
                txtLastName.Text = thisPrisoner.LastName;
                txtID.Text = thisPrisoner.SA_ID;
                ckbCleared.Checked = thisPrisoner.ClearedToWork == true ? true : false;
            }
            else
            {
                MessageBox.Show("Please select a Prisoner Number");
            }
        }

        private void lstMPAllSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isPopulating) return;

            skillID = ListboxHelper.GetSelectedSkillID(lstMPAllSkills);
        }

        private void btnMPAdd_Click(object sender, EventArgs e)
        {
            if (skillID != -1 && thisPrisoner != null)
            {
                PrisonerSkillManager.AddPrisonerSkill(thisPrisoner.PrisonerID, skillID);
                skillID = -1;
                isPopulating = true;
                ListboxHelper.PopulateAvailablePSkills(lstMPAllSkills, thisPrisoner.PrisonerID);
                ListboxHelper.PopulatePrisonerSkills(lstMPCurrentSkills, thisPrisoner.PrisonerID);
                isPopulating = false;
            }
            else if (skillID == -1)
            {
                MessageBox.Show("Please select a skill", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (thisPrisoner == null)
            {
                MessageBox.Show("Please select a prisoner", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMPRemove_Click(object sender, EventArgs e)
        {
            if (skillID != -1 && thisPrisoner != null)
            {
                PrisonerSkillManager.RemovePrisonerSkill(thisPrisoner.PrisonerID, skillID);
                skillID = -1;
                isPopulating = true;
                ListboxHelper.PopulateAvailablePSkills(lstMPAllSkills, thisPrisoner.PrisonerID);
                ListboxHelper.PopulatePrisonerSkills(lstMPCurrentSkills, thisPrisoner.PrisonerID);
                isPopulating = false;
            }
            else if (skillID == -1)
            {
                MessageBox.Show("Please select a skill", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (thisPrisoner == null)
            {
                MessageBox.Show("Please select a prisoner", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstMPCurrentSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isPopulating) return;

            skillID = ListboxHelper.GetSelectedSkillID(lstMPCurrentSkills);
        }



        //Edit Prisoner
        private void button46_Click(object sender, EventArgs e)
        {
            btnMPSave.Text = "Save";
            isupdatingPrisoner = true;

            txtID.Enabled = false;
            pnlPInfo.Visible = true;
            pnlMPSkills.Visible = true;
            cmbMPPrisonerNo.Visible = true;
            txtPNo.Visible = false;
            pnlMPChoices.Visible = false;
            txtCurrentPassword.Enabled = true;
        }

        //Delete Prisoner
        private void button47_Click(object sender, EventArgs e)
        {
            btnMPSave.Text = "Delete";

            isDeletingPrisoner = true;

            txtID.Enabled = false;
            pnlPInfo.Visible = true;
            pnlMPSkills.Visible = true;
            cmbMPPrisonerNo.Visible = true;
            txtPNo.Visible = false;
            pnlMPChoices.Visible = false;
            txtCurrentPassword.Enabled = true;
        }

        //Add Prisoner
        private void button53_Click(object sender, EventArgs e)
        {
            btnMPSave.Text = "Add";

            isAddingPrisoner = true;

            txtID.Enabled = true;
            pnlPInfo.Visible = true;
            pnlMPSkills.Visible = true;
            cmbMPPrisonerNo.Visible = false;
            txtPNo.Visible = true;
            pnlMPChoices.Visible = false;
            txtCurrentPassword.Enabled = false;

            isPopulating = true;
            ListboxHelper.PopulateAllPSkills(lstMPAllSkills);
            isPopulating = false;

            pnlMPSkills.Enabled = false;
        }

        private void btnMPSave_Click(object sender, EventArgs e)
        {
            if (isAddingPrisoner && pnlMPSkills.Enabled == false)
            {
                thisPrisoner = new Prisoner();

                thisPrisoner.PrisonerNum = Prisoner.GeneratePrisonerNum();
                thisPrisoner.LastName = txtLastName.Text.Trim();
                thisPrisoner.FirstName = txtFirstName.Text.Trim();
                thisPrisoner.SA_ID = txtID.Text.Trim();
                thisPrisoner.Password = Ppassword;
                thisPrisoner.ClearedToWork = ckbCleared.Checked;

                if (thisPrisoner.ValidateNewPrisoner())
                {
                    thisPrisoner.PrisonerID = PrisonerManager.AddPrisoner(thisPrisoner);
                    if (thisPrisoner.PrisonerID > 0)
                    {
                        MessageBox.Show("Prisoner " + thisPrisoner.PrisonerNum + " successfully added! Now you can manage their skills");

                        lstMPAllSkills.SelectedIndexChanged -= lstMPAllSkills_SelectedIndexChanged;
                        ListboxHelper.PopulateAvailablePSkills(lstMPAllSkills, thisPrisoner.PrisonerID);
                        lstMPAllSkills.SelectedIndexChanged += lstMPAllSkills_SelectedIndexChanged;
                    }
                }
                else
                {
                    return;
                }

                pnlMPSkills.Enabled = true;
                return;
            }
            else if (isupdatingPrisoner)
            {
                thisPrisoner.LastName = txtLastName.Text.Trim();
                thisPrisoner.FirstName = txtFirstName.Text.Trim();
                thisPrisoner.SA_ID = txtID.Text.Trim();
                thisPrisoner.Password = Ppassword;
                thisPrisoner.ClearedToWork = ckbCleared.Checked;

                if (thisPrisoner.ValidateNewPrisoner())
                {
                    bool pUpdated = PrisonerManager.UpdatePrisoner(thisPrisoner);
                    if (pUpdated)
                    {
                        MessageBox.Show("Prisoner " + thisPrisoner.PrisonerNum + " updated successfully!");
                    }
                }
                else
                {
                    return;
                }
            }
            else if (isDeletingPrisoner)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this prisoner?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // User clicked Yes - proceed with deletion
                    if (PrisonerManager.DeletePrisoner(thisPrisoner.PrisonerID))
                    {
                        MessageBox.Show("Prisoner " + thisPrisoner.PrisonerNum + " deleted successfully.");
                        pnlPInfo.Enabled = true;
                        pnlModifyPPassword.Enabled = true;
                        pnlMPSkills.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Deletion could not proceed. Try again");
                    }
                }
                else
                {
                    // User clicked No - cancel deletion
                    MessageBox.Show("Delete cancelled.");
                }
            }

            if (isAddingPrisoner)
            {
                MessageBox.Show("Prisoner data saved!!");
            }

            btnMPSave.Text = "Save";

            lstMPAllSkills.DataSource = null;
            lstMPAllSkills.Items.Clear();
            lstMPCurrentSkills.DataSource = null;
            lstMPCurrentSkills.Items.Clear();

            pnlPInfo.Visible = false;
            pnlMPSkills.Visible = false;
            cmbMPPrisonerNo.Visible = false;
            pnlMPChoices.Visible = true;

            cmbMPPrisonerNo.SelectedIndexChanged -= cmbMPPrisonerNo_SelectedIndexChanged;
            ComboBoxHelper.PopulatePrisonerNumbers(cmbMPPrisonerNo);
            cmbMPPrisonerNo.SelectedIndexChanged += cmbMPPrisonerNo_SelectedIndexChanged;

            isAddingPrisoner = false;
            isDeletingPrisoner = false;
            isupdatingPrisoner = false;
            Ppassword = "";

        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            pnlModifyPPassword.Visible = true;
            pnlPInfo.Visible = false;
        }

        private void btnSavePassword_Click(object sender, EventArgs e)
        {
            if (isAddingPrisoner)
            {
                if (txtNewPassword.Text == txtConfirmPassword.Text && txtNewPassword.Text != "")
                {
                    if (txtNewPassword.Text.Trim().Length < 6)
                    {
                        MessageBox.Show("Password must be at least 6 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    MessageBox.Show("Saving new password");
                    Ppassword = txtNewPassword.Text;
                }
                else
                {
                    MessageBox.Show("Please enter the same password in both open spaces", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (isupdatingPrisoner)
            {
                if (txtCurrentPassword.Text.Trim() == thisPrisoner.Password)
                {
                    if (txtNewPassword.Text == txtConfirmPassword.Text && txtNewPassword.Text != "")
                    {
                        if (txtNewPassword.Text.Trim().Length < 6)
                        {
                            MessageBox.Show("Password must be at least 6 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        MessageBox.Show("Saving new password");
                        Ppassword = txtNewPassword.Text;
                    }
                    else
                    {
                        MessageBox.Show("Please enter the same password in both open spaces", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Wrong password. Password Change denied", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            }

            pnlModifyPPassword.Visible = false;
            pnlPInfo.Visible = true;


        }

        private void btnCompleteJobs_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 2;
        }

        private void btnMaintainSkills_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 6;
        }

        private void btnMSEditExisSkil_Click(object sender, EventArgs e)
        {
            // Set operation mode and reset state
            skillOperation = "EDIT";
            selectedSkillID = -1;
            btnMSSave.Text = "Save";

            // Show skill selection panel only
            panel19.Visible = true;
            panel20.Visible = false;

            // preventing a skill to be automatically selected.
            isPopulating = true;
            ComboBoxHelper.PopulateSkillNames(cbxMSSkillName);
            cbxMSSkillName.SelectedIndex = -1;  // ensuring no skill is selected
            isPopulating = false;

            // keeping the fields clear until the user selects a skill
            txtMSSkillName.Clear();
            txtMSSkillDesc.Clear();

            // Enable skill selection combobox for editing
            cbxMSSkillName.Enabled = true;
        }


        private void cbxMSSkillName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isPopulating || cbxMSSkillName.SelectedIndex == -1) return;

            // User crash prevention: Only allow skill loading if in EDIT or DELETE mode
            if (skillOperation != "EDIT" && skillOperation != "DELETE")
            {
                MessageBox.Show("Please select an operation first (Edit or Delete).");
                cbxMSSkillName.SelectedIndex = -1;
                return;
            }

            selectedSkillID = ComboBoxHelper.GetSelectedID(cbxMSSkillName);

            using (SqlConnection conn = new SqlConnection(DatabaseConfig.GetConStr()))
            {
                string query = "SELECT SkillName, SkillDescription FROM SKILL WHERE SkillID = @SkillID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SkillId", selectedSkillID);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtMSSkillName.Text = reader["SkillName"].ToString();
                        txtMSSkillDesc.Text = reader["SkillDescription"].ToString();

                        // Show editing panel only after skill is loaded
                        panel20.Visible = true;

                        // Set field access based on operation
                        if (skillOperation == "DELETE")
                        {
                            // Make fields read-only for delete operation
                            txtMSSkillName.ReadOnly = true;
                            txtMSSkillDesc.ReadOnly = true;
                        }
                        else if (skillOperation == "EDIT")
                        {
                            // Make fields editable for edit operation
                            txtMSSkillName.ReadOnly = false;
                            txtMSSkillDesc.ReadOnly = false;
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error receiving skill: " + ex.Message);
                }
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            string skillName = txtMSSkillName.Text.Trim();
            string skillDesc = txtMSSkillDesc.Text.Trim();


            if (skillOperation == "DELETE")
            {
                if (selectedSkillID == -1)
                {
                    MessageBox.Show("Please select a skill to delete first.");
                    return;
                }
                PerformSkillDeletion();
                return;
            }

            if (string.IsNullOrEmpty(skillName))
            {
                MessageBox.Show("Please enter a name for the skill!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(DatabaseConfig.GetConStr()))
            {
                SqlCommand cmd;

                if (selectedSkillID == -1) // new skills 
                {
                    // Ensuring that there is a skill name and description
                    if (txtMSSkillName.Text == "" || txtMSSkillDesc.Text == "")
                    {
                        MessageBox.Show("Please ensure that both the skill name and description are entered");
                    }
                    else
                    {
                        cmd = new SqlCommand("INSERT INTO SKILL (SkillName, SkillDescription) OUTPUT INSERTED.SkillID VALUES (@Name, @Desc)", conn);
                        cmd.Parameters.AddWithValue("@Name", skillName);
                        cmd.Parameters.AddWithValue("@Desc", skillDesc);

                        try
                        {
                            conn.Open();
                            int newSkillID = (int)cmd.ExecuteScalar();

                            // optionally linking the skill to the prisoner.
                            int defaultPrisonerID = prsnrID != 0 ? prsnrID : 1;
                            SqlCommand linkCmd = new SqlCommand("INSERT INTO SKILL_OF_PRISONER (PrisonerID, SkillID) VALUES (@PrisonerID, @SkillID)", conn);
                            linkCmd.Parameters.AddWithValue("@PrisonerID", defaultPrisonerID);
                            linkCmd.Parameters.AddWithValue("@SkillID", newSkillID);
                            linkCmd.ExecuteNonQuery();

                            MessageBox.Show("Skill added successfully!");
                            // Reset state after successful add
                            ResetMaintainSkillsState();


                            // ensuring the textboxes are clear after a skill has been added.
                            txtMSSkillName.Clear();
                            txtMSSkillDesc.Clear();
                            selectedSkillID = -1;

                            // preventing the auto trigerring
                            isPopulating = true;
                            ComboBoxHelper.PopulateSkillNames(cbxMSSkillName);
                            cbxMSSkillName.SelectedIndex = -1;
                            isPopulating = false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error saving skill: " + ex.Message);
                        }
                    }
                }
                else // updating the existing skill.
                {
                    cmd = new SqlCommand("UPDATE SKILL SET SkillName = @Name, SkillDescription = @Desc WHERE SkillID = @SkillID", conn);
                    cmd.Parameters.AddWithValue("@Name", skillName);
                    cmd.Parameters.AddWithValue("@Desc", skillDesc);
                    cmd.Parameters.AddWithValue("@SkillID", selectedSkillID);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Skill updated successfully!");
                        // Reset state after successful update
                        ResetMaintainSkillsState();

                        // clearing the textboxes after the skill has been updated.
                        txtMSSkillName.Clear();
                        txtMSSkillDesc.Clear();
                        selectedSkillID = -1;

                        isPopulating = true;
                        ComboBoxHelper.PopulateSkillNames(cbxMSSkillName);
                        cbxMSSkillName.SelectedIndex = -1;
                        isPopulating = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error updating skill: " + ex.Message);
                    }
                }
            }
        }


        private void btnMSDelExisSkill_Click(object sender, EventArgs e)
        {
            btnMSSave.Text = "Delete";
            // Set operation mode
            skillOperation = "DELETE";

            // Show skill selection panel
            panel19.Visible = true;
            panel20.Visible = false;

            // Populate combobox for skill selection
            isPopulating = true;
            ComboBoxHelper.PopulateSkillNames(cbxMSSkillName);
            cbxMSSkillName.SelectedIndex = -1; // ensuring that there is no skill auto selected.
            isPopulating = false;

            // Enable combobox for selection
            cbxMSSkillName.Enabled = true;

            // Clear textboxes and reset selected skill
            txtMSSkillName.Clear();
            txtMSSkillDesc.Clear();
            selectedSkillID = -1;

        }



        private void btnMSNext_Click(object sender, EventArgs e)
        {
            // Only show panel20 if we're in ADD mode or have a skill selected for EDIT/DELETE
            if (skillOperation == "ADD" || (selectedSkillID != -1 && (skillOperation == "EDIT" || skillOperation == "DELETE")))
            {
                panel20.Visible = true;
            }
            else
            {
                MessageBox.Show("Please select an operation and skill first.");
            }
        }

        private void btnMSAddNewSkill_Click(object sender, EventArgs e)
        {
            // Set operation mode
            skillOperation = "ADD";
            selectedSkillID = -1;
            btnMSSave.Text = "Save";

            // Hide skill selection panel, show editing panel
            panel19.Visible = false;
            panel20.Visible = true;

            // Clear fields for new skill entry
            txtMSSkillName.Clear();
            txtMSSkillDesc.Clear();

            // Make fields editable
            txtMSSkillName.ReadOnly = false;
            txtMSSkillDesc.ReadOnly = false;
        }
        private void PerformSkillDeletion()
        {
            if (selectedSkillID == -1)
            {
                MessageBox.Show("Please select a skill to delete.");
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this skill? This will also remove it from all prisoners and jobs.",
                                                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConfig.GetConStr()))
                {
                    try
                    {
                        conn.Open();

                        // first deleting the skill from SKILL_OF_PRISONER
                        SqlCommand linkCmd = new SqlCommand("DELETE FROM SKILL_OF_PRISONER WHERE SkillID = @SkillID", conn);
                        linkCmd.Parameters.AddWithValue("@SkillID", selectedSkillID);
                        linkCmd.ExecuteNonQuery();

                        // secondly deleting skill from SKILL_OF_JOB
                        SqlCommand linkCmd2 = new SqlCommand("DELETE FROM SKILL_OF_JOB WHERE SkillID = @SkillID", conn);
                        linkCmd2.Parameters.AddWithValue("@SkillID", selectedSkillID);
                        linkCmd2.ExecuteNonQuery();

                        // Delete from SKILL
                        SqlCommand cmd = new SqlCommand("DELETE FROM SKILL WHERE SkillID = @SkillID", conn);
                        cmd.Parameters.AddWithValue("@SkillID", selectedSkillID);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Skill deleted successfully!");

                        // Reset state after successful deletion
                        ResetMaintainSkillsState();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting skill: " + ex.Message);
                    }
                }
            }
        }

        // Helper method to reset maintain skills state
        private void ResetMaintainSkillsState()
        {
            // clearing textboxes
            txtMSSkillName.Clear();
            txtMSSkillDesc.Clear();
            selectedSkillID = -1;
            skillOperation = "";

            // Reset field access
            txtMSSkillName.ReadOnly = false;
            txtMSSkillDesc.ReadOnly = false;

            // Hide panels
            panel19.Visible = false;
            panel20.Visible = false;

            // Reset combobox
            isPopulating = true;
            ComboBoxHelper.PopulateSkillNames(cbxMSSkillName);
            cbxMSSkillName.SelectedIndex = -1;
            isPopulating = false;
        }

        private void btnEmployerDetails_Click(object sender, EventArgs e)
        {
            //show panel to edit employer details with data auto filled in
            if (cmbEmpDetails.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an Employer to edit details.");
                return;
            }
            else
            {



                int employerId = Convert.ToInt32(cmbEmpDetails.SelectedValue);

                using (SqlConnection conn = new SqlConnection(DatabaseConfig.GetConStr()))
                {
                    conn.Open();

                    string sql = "SELECT EmployerID, FirstName, LastName FROM EMPLOYER WHERE EmployerID = @EmployerID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployerID", employerId);

                        try
                        {

                            SqlDataReader reader = cmd.ExecuteReader();

                            if (reader.Read())
                            {
                                // Fill textboxes
                                txtEmpID.Text = reader["EmployerID"].ToString();
                                txtEmpFName.Text = reader["FirstName"].ToString();
                                txtEmpLName.Text = reader["LastName"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("No employer found with that ID.");
                            }

                            reader.Close();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error loading employer details: " + ex.Message);
                        }
                    }
                    conn.Close();

                }

                PnlEmpInfo.Visible = true;
                btnEmpChangePass.Visible = true;
                btnAddNewEmp.Visible = false;
                txtEmpFName.Enabled = false;
                txtEmpLName.Enabled = false;

                pnlEmpDetails.Visible = false;
                txtEmpPass.Visible = false;
                lblEmpPass.Visible = false;
                btnEmpEdit.Visible = true;
                imgMEPass.Visible = false;


            }
        }



        private void btnAddEmp_Click(object sender, EventArgs e)
        {
            //show panel to ley user insert new employer 
            pnlEmpDetails.Visible = false;
            PnlEmpInfo.Visible = true;
            btnAddNewEmp.Visible = true;
            btnEmpChangePass.Visible = false;
            pnlEmpMenu.Visible = false;
            btnBackEmp.Visible = true;
            txtEmpID.Clear();
            txtEmpFName.Clear();
            txtEmpLName.Clear();
            txtEmpPass.Clear();
            txtEmpFName.Enabled = true;
            txtEmpLName.Enabled = true;
            lblEmpPass.Visible = true;
            txtEmpPass.Visible = true;
            imgMEPass.Visible = true;
        }

        private void btnEditEmp_Click(object sender, EventArgs e)
        {
            //show panel for user to select employer to edit

            pnlEmpDetails.Visible = true;
            pnlEmpMenu.Visible = false;
            btnEmployerDetails.Visible = true;
            btnEmpDel.Visible = false;
            btnBackEmp.Visible = true;
        }

        private void btnDelEmployer_Click(object sender, EventArgs e)
        {
            //show panel for user to select employer to delete
            pnlEmpDetails.Visible = true;

            pnlEmpMenu.Visible = false;
            btnEmpDel.Visible = true;
            btnEmployerDetails.Visible = false;
            btnBackEmp.Visible = true;


        }

        private void btnBackEmp_Click(object sender, EventArgs e)
        {
            pnlEmpMenu.Visible = true;
            PnlEmpInfo.Visible = false;
            pnlEmpDetails.Visible = false;
            btnEmpEdit.Visible = false;
            pnlEmpPass.Visible = false;
            btnBackEmp.Visible = false;
            btnEmpSave.Visible = false;
        }

        private void btnEmpChangePass_Click(object sender, EventArgs e)
        {
            //show the panel to change password
            pnlEmpPass.Visible = true;
        }

        private void btnEmpCancel_Click(object sender, EventArgs e)
        {
            pnlEmpPass.Visible = false;
            txtEmpNew.Clear();
            txtEmpConfirm.Clear();

        }



        private void btnClear_Click(object sender, EventArgs e)
        {
            //Clearing all the Job completed details
            cbxCJJobName.SelectedIndex = -1;
            calDateCompleted.SetDate(DateTime.Today);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {

                if (cbxCJJobName.SelectedIndex != -1)
                {
                    DateTime selectedDate = calDateCompleted.SelectionStart;
                    if (selectedDate <= DateTime.Today)
                    {
                        string connStr = DatabaseConfig.GetConStr();

                        SqlConnection conn = new SqlConnection(connStr);

                        conn.Open();
                        string getIdSql = "SELECT JobID FROM JOB WHERE JobName = @JobName";
                        SqlCommand getIdcomm = new SqlCommand(getIdSql, conn);
                        getIdcomm.Parameters.AddWithValue("@JobName", cbxCJJobName.Text);
                        int jobId = (int)getIdcomm.ExecuteScalar();


                        string sql = "UPDATE JOB SET EndDate = @EndDate, Completed = 1 WHERE JobID = @JobID; UPDATE PRISONER_ON_JOB SET JobFinished = 1 WHERE JobID = @JobID";
                        SqlCommand comm = new SqlCommand(sql, conn);

                        comm.Parameters.AddWithValue("@EndDate", selectedDate);
                        comm.Parameters.AddWithValue("@JobID", jobId);
                        comm.ExecuteNonQuery();


                        ComboBoxHelper.PopulateUncompletedJobNames(cbxCJJobName);

                        //Clearing all the Job completed details
                        cbxCJJobName.SelectedIndex = -1;
                        calDateCompleted.SetDate(DateTime.Today);

                        MessageBox.Show("Job completed has been updated.");



                        conn.Close();

                    }
                    else
                    {
                        MessageBox.Show("Please select an date that is either today or in the past.");
                    }
                }
                else
                {
                    MessageBox.Show("Please select an Job.");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void btnAssignGuardsToJobs_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 4;
        }

        private void btnAssignSkillToPris_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 7;
        }

        private void btnAssignSkillToJob_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 5;
        }

        private void btnMaintainEmp_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 1;
        }

        private void btnMaintainPris_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 8;
        }

        private void btnReqReports_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmReports ReportForm = new frmReports();
            ReportForm.ShowDialog();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            pnlModifyPPassword.Visible = false;
            pnlPInfo.Visible = true;
        }

        private void btnASPAdd_Click(object sender, EventArgs e)
        {
            if (skillID != -1 && thisPrisoner != null)
            {
                PrisonerSkillManager.AddPrisonerSkill(thisPrisoner.PrisonerID, skillID);
                skillID = -1;
                isPopulating = true;
                ListboxHelper.PopulateAvailablePSkills(lstASPAllSkills, thisPrisoner.PrisonerID);
                ListboxHelper.PopulatePrisonerSkills(lstASPCurrentSkills, thisPrisoner.PrisonerID);
                isPopulating = false;
            }
            else if (skillID == -1)
            {
                MessageBox.Show("Please select a skill", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (thisPrisoner == null)
            {
                MessageBox.Show("Please select a prisoner", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void pnlEmpDetails_VisibleChanged(object sender, EventArgs e)
        {
            //Populating the combobox with employer names on the visible change event of the pnlEmpDetails panel
            ComboBoxHelper.PopulateEmployerNames(cmbEmpDetails);
        }

        private void btnEmpDel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete this prisoner?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                if (cmbEmpDetails.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select an employer!");
                    return;
                }

                // Get EmployerID directly from ValueMember
                int employerId = Convert.ToInt32(cmbEmpDetails.SelectedValue);

                using (SqlConnection conn = new SqlConnection(DatabaseConfig.GetConStr()))
                {
                    conn.Open();

                    string sql1 = "SELECT JobID FROM JOB WHERE EmployerID = @ID";
                    using (SqlCommand cmd1 = new SqlCommand(sql1, conn))
                    {
                        cmd1.Parameters.AddWithValue("@ID", employerId);
                        SqlDataReader reader = cmd1.ExecuteReader();
                        List<int> jobIds = new List<int>();
                        while (reader.Read())
                        {
                            jobIds.Add(reader.GetInt32(0));
                        }
                        reader.Close();
                        // Delete associated records in PRISONER_ON_JOB for each job
                        foreach (int jobId in jobIds)
                        {
                            string sql2 = "DELETE FROM PRISONER_ON_JOB WHERE JobID = @JobID";
                            using (SqlCommand cmd2 = new SqlCommand(sql2, conn))
                            {
                                cmd2.Parameters.AddWithValue("@JobID", jobId);
                                cmd2.ExecuteNonQuery();
                            }

                            string sql3 = "DELETE FROM SKILL_OF_JOB WHERE JobID = @JobID";
                            using (SqlCommand cmd3 = new SqlCommand(sql3, conn))
                            {
                                cmd3.Parameters.AddWithValue("@JobID", jobId);
                                cmd3.ExecuteNonQuery();
                            }
                        }
                    }
                    string sql4 = "DELETE FROM JOB Where EmployerID = @ID";
                    using (SqlCommand cmd4 = new SqlCommand(sql4, conn))
                    {
                        cmd4.Parameters.AddWithValue("@ID", employerId);
                        cmd4.ExecuteNonQuery();
                    }
                    string sql5 = "DELETE FROM EMPLOYER WHERE EmployerID = @ID";
                    using (SqlCommand cmd5 = new SqlCommand(sql5, conn))
                    {
                        cmd5.Parameters.AddWithValue("@ID", employerId);

                        int rowsAffected = cmd5.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            MessageBox.Show("Employer deleted successfully!");
                        else
                            MessageBox.Show("No matching employer found!");
                    }
                }

                // Refresh the ComboBox after deletion
                ComboBoxHelper.PopulateEmployerNames(cmbEmpDetails);

                cmbEmpDetails.SelectedIndex = -1;
                btnEmpDel.Visible = false;
                pnlEmpDetails.Visible = false;
                pnlEmpMenu.Visible = true;
                btnBackEmp.Visible = false;
            }
            else
            {
                // User clicked No - cancel deletion
                MessageBox.Show("Delete cancelled.");
            }//delete employer

        }



        private void btnAddNewEmp_Click(object sender, EventArgs e)
        {
            //Add new employer
            if (txtEmpFName.Text == "" || txtEmpLName.Text == "" || txtEmpPass.Text == "")
            {
                MessageBox.Show("Please enter all the details for the employer!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (txtEmpPass.Text.Trim().Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string firstName = txtEmpFName.Text;
            string lastName = txtEmpLName.Text;
            string password = PasswordHasser.HashPassword(txtEmpPass.Text);



            SqlConnection conn = new SqlConnection(DatabaseConfig.GetConStr());
            conn.Open();
            string sql = "INSERT INTO EMPLOYER ( FirstName, LastName,Password) VALUES (@FName, @LName, @password)";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@FName", firstName);
                cmd.Parameters.AddWithValue("@LName", lastName);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Employer added successfully!");
            conn.Close();
            txtEmpFName.Clear();
            txtEmpLName.Clear();
            txtEmpPass.Clear();
            btnAddNewEmp.Visible = false;
            PnlEmpInfo.Visible = false;
            pnlEmpMenu.Visible = true;
            btnBackEmp.Visible = false;
            imgMEPass.Visible = false;


        }

        private void pnlMPChoices_VisibleChanged(object sender, EventArgs e)
        {
            if (pnlMPChoices.Visible == true)
            {
                txtPNo.Clear();
                txtFirstName.Clear();
                txtLastName.Clear();
                txtID.Clear();
                ckbCleared.Checked = false;
                lstMPCurrentSkills.ClearSelected();
                lstMPAllSkills.ClearSelected();
                txtCurrentPassword.Clear();
                txtNewPassword.Clear();
                txtConfirmPassword.Clear();


                btnMPSave.Visible = false;
                btnMPBack.Visible = false;

                cmbMPPrisonerNo.SelectedIndexChanged -= cmbMPPrisonerNo_SelectedIndexChanged;
                cmbMPPrisonerNo.SelectedIndex = -1;
                cmbMPPrisonerNo.SelectedIndexChanged += cmbMPPrisonerNo_SelectedIndexChanged;

                pnlPInfo.Enabled = true;
                pnlModifyPPassword.Enabled = true;
                pnlMPSkills.Enabled = true;
            }
            else if (!pnlMPChoices.Visible)
            {
                btnMPSave.Visible = true;
                btnMPBack.Visible = true; ;
            }
        }

        private void cmbPrisonerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            prsnrID = ComboBoxHelper.GetSelectedID(cmbPrisonerName);
            thisPrisoner = PrisonerManager.LoadPrisoner(prsnrID);

            isPopulating = true;
            ListboxHelper.PopulateAvailablePSkills(lstASPAllSkills, thisPrisoner.PrisonerID);
            ListboxHelper.PopulatePrisonerSkills(lstASPCurrentSkills, thisPrisoner.PrisonerID);
            isPopulating = false;

        }

        private void btnASPRemove_Click(object sender, EventArgs e)
        {
            if (skillID != -1 && thisPrisoner != null)
            {
                PrisonerSkillManager.RemovePrisonerSkill(thisPrisoner.PrisonerID, skillID);
                skillID = -1;
                isPopulating = true;
                ListboxHelper.PopulateAvailablePSkills(lstASPAllSkills, thisPrisoner.PrisonerID);
                ListboxHelper.PopulatePrisonerSkills(lstASPCurrentSkills, thisPrisoner.PrisonerID);
                isPopulating = false;
            }
            else if (skillID == -1)
            {
                MessageBox.Show("Please select a skill", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (thisPrisoner == null)
            {
                MessageBox.Show("Please select a prisoner", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstASPAllSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isPopulating) return;

            skillID = ListboxHelper.GetSelectedSkillID(lstASPAllSkills);
        }

        private void button40_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Skills of Prisoner saved!");

            cmbPrisonerName.SelectedIndexChanged -= cmbPrisonerName_SelectedIndexChanged;
            cmbPrisonerName.SelectedIndex = -1;
            cmbPrisonerName.SelectedIndexChanged += cmbPrisonerName_SelectedIndexChanged;

            lstASPAllSkills.DataSource = null;
            lstASPAllSkills.Items.Clear();
            lstASPCurrentSkills.DataSource = null;
            lstASPCurrentSkills.Items.Clear();

        }

        private void button41_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to return to the main menu?", "Confirm Return", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                cmbPrisonerName.SelectedIndexChanged -= cmbPrisonerName_SelectedIndexChanged;
                cmbPrisonerName.SelectedIndex = -1;
                cmbPrisonerName.SelectedIndexChanged += cmbPrisonerName_SelectedIndexChanged;

                lstASPAllSkills.DataSource = null;
                lstASPAllSkills.Items.Clear();
                lstASPCurrentSkills.DataSource = null;
                lstASPCurrentSkills.Items.Clear();

                frmLogIn loginForm = new frmLogIn();
                this.Close();
                loginForm.ShowDialog();
            }
            else
            {

            }
        }

        private void lstASPCurrentSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isPopulating) return;

            skillID = ListboxHelper.GetSelectedSkillID(lstASPCurrentSkills);
        }
        private string GetEmployerNameById(int employerID)
        {
            try
            {
                string connStr = DatabaseConfig.GetConStr();
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = @"SELECT CONCAT(FirstName, ' ', LastName) AS FullName FROM EMPLOYER WHERE EmployerID = @EmployerID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployerID", employerID);
                        conn.Open();

                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return result.ToString();
                        }
                        else
                        {
                            MessageBox.Show($"Employer with ID {employerID} not found.");
                            return string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving employer name: {ex.Message}");
                return string.Empty;
            }
        }

        private void button32_Click_1(object sender, EventArgs e)
        {
            // Add job button
            try
            {
                // Making panels visible
                btnMJSave.Text = "Add";

                pnlAdSkills.Enabled = false;
                cbxMJEmpName.SelectedIndexChanged -= cbxMJEmpName_SelectedIndexChanged;
                ComboBoxHelper.PopulateEmployerNames(cbxMJEmpName);
                cbxMJEmpName.SelectedIndexChanged += cbxMJEmpName_SelectedIndexChanged;

                pnladJobDetails.Visible = true;
                pnlAdDate.Visible = true;
                pnlAdFunctions.Visible = false;
                pnlAdSkills.Visible = true;
                cmbJobNAme.Visible = false;
                txtJobname.Visible = true;

                // Clear all fields for new job
                txtJobname.Clear();
                rtbJobDescription.Clear();
                rtbJobLocation.Clear();
                calStartDate.SelectionStart = DateTime.Today;
                calEndDate.SelectionStart = DateTime.Today.AddDays(1);
                nudDuration.Value = 8;
                cbJobCompleted.Checked = false;
                sbxPNeeded.Value = 1;

                selectedJobID = -1;
                action = "ADD";

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }

        }

        private void button30_Click_1(object sender, EventArgs e)
        {
            // Edit existing job
            try
            {
                // Making panels visible
                cbxMJEmpName.SelectedIndexChanged -= cbxMJEmpName_SelectedIndexChanged;
                cbxMJEmpName.DataSource = null;
                cbxMJEmpName.Items.Clear();
                cbxMJEmpName.SelectedIndexChanged += cbxMJEmpName_SelectedIndexChanged;

                btnMJSave.Text = "Save";

                pnladJobDetails.Visible = true;
                pnlAdDate.Visible = true;
                pnlAdFunctions.Visible = false;
                pnlAdSkills.Visible = true;
                cmbJobNAme.Visible = true;
                txtJobname.Visible = false;

                ComboBoxHelper.PopulateJobNames(cmbJobNAme);

                action = "EDIT";
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }


        private void button28_Click(object sender, EventArgs e)
        {

            // Save button  
            try
            {
                // Setting values into variables
                string job_name = txtJobname.Text.Trim();
                string job_description = rtbJobDescription.Text.Trim();
                string job_location = rtbJobLocation.Text.Trim();
                DateTime startDate = calStartDate.SelectionStart;
                DateTime endDate = calEndDate.SelectionStart;
                decimal duration = nudDuration.Value;
                bool job_completed = cbJobCompleted.Checked;
                int prisoners = (int)sbxPNeeded.Value;

                // Validation
                if (string.IsNullOrEmpty(job_name))
                {
                    MessageBox.Show("Job name is required.");
                    return;
                }

                if (string.IsNullOrEmpty(job_description))
                {
                    MessageBox.Show("Job description is required.");
                    return;
                }

                if (string.IsNullOrEmpty(job_location))
                {
                    MessageBox.Show("Job location is required.");
                    return;
                }

                if (startDate >= endDate)
                {
                    MessageBox.Show("End date must be after start date.");
                    return;
                }

                // Add job
                if (action == "ADD" && !pnlAdSkills.Enabled)
                {

                    int employerID = ComboBoxHelper.GetSelectedID(cbxMJEmpName);
                    if (employerID <= 0)
                    {
                        MessageBox.Show("Invalid employer selected. Please check the employer name.");
                        return;
                    }

                    Job newJob = new Job
                    {
                        EmployerID = employerID, JobName = job_name, JobDescription = job_description, JobLocation = job_location, StartDate = startDate, EndDate = endDate, PrisonersNeeded = (int)prisoners, Completed = job_completed, DurationPerDay = (int)duration
                    };

                    int newJobID = JobManager.AddJob(newJob);

                    if (newJobID > 0)
                    {
                        MessageBox.Show($"Job added successfully with ID: {newJobID}. Now you can assign skills to the job");
                        selectedJobID = newJobID;
                        pnlAdSkills.Enabled = true;

                        ListboxHelper.PopulateCurrentJobSkills(lstMJCurrentSkills, selectedJobID);
                        ListboxHelper.PopulateAvailablePSkills(lstMJAllSkills, selectedJobID);

                        return;
                    }
                    else
                    {
                        MessageBox.Show("Failed to add job.");
                        return;
                    }
                }
                else if(action == "ADD" && pnlAdSkills.Enabled) 
                {
                    MessageBox.Show($"Job added successfully.");

                }
                // Edit job
                else if (action == "EDIT")
                {
                    int employerID = selectedJob.EmployerID;

                    if (selectedJobID <= 0)
                    {
                        MessageBox.Show("Please select a job to edit.");
                        return;
                    }

                    bool success = JobManager.UpdateJob(selectedJobID, employerID, job_name, job_description, job_location, startDate, endDate, duration, job_completed, prisoners);

                    if (success)
                    {
                        MessageBox.Show("Job updated successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to update job.");
                        return;
                    }
                }
                // Delete job
                else if (action == "DELETE")
                {
                    if (selectedJobID <= 0)
                    {
                        MessageBox.Show("Please select a job to delete.");
                        return;
                    }
                    else
                    {
                        JobManager.DeleteJob(selectedJobID);
                        MessageBox.Show("Job successfully deleted");
                    }


                }

                ResetJobForm();

                pnladJobDetails.Visible = false;
                pnlAdDate.Visible = false;
                pnlAdFunctions.Visible = true;
                pnlAdSkills.Visible = false;

                ComboBoxHelper.PopulateJobNames(cmbJobNAme);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void ResetJobForm()
        {
            cbxMJEmpName.Enabled = true;
            sbxPNeeded.Enabled = true;
            pnlAdDate.Enabled = true;

            lstMJAllSkills.DataSource = null;
            lstMJAllSkills.Items.Clear();
            lstMJCurrentSkills.DataSource = null;
            lstMJCurrentSkills.Items.Clear();

            pnlAdSkills.Enabled = true;
            btnMJSave.Text = "Save";
            txtJobname.Clear();
            rtbJobDescription.Clear();
            rtbJobLocation.Clear();
            calStartDate.SelectionStart = DateTime.Today;
            calEndDate.SelectionStart = DateTime.Today.AddDays(1);
            nudDuration.Value = 8;
            cbJobCompleted.Checked = false;
            sbxPNeeded.Value = 1;
            selectedJobID = -1;
            action = "";

            cbxMJEmpName.SelectedIndexChanged -= cbxMJEmpName_SelectedIndexChanged;
            cbxJobName.Enabled = true;
            cbxMJEmpName.DataSource = null;
            cbxMJEmpName.Items.Clear();
            cbxMJEmpName.SelectedIndexChanged += cbxMJEmpName_SelectedIndexChanged;

            txtJobname.ReadOnly = false;
            rtbJobDescription.ReadOnly = false;
            rtbJobLocation.ReadOnly = false;


            cmbJobNAme.SelectedIndex = -1;
        }

        
        private void button31_Click(object sender, EventArgs e)
        {
            // Delete existing job
            try
            {
                // Making panels visible
                btnMJSave.Text = "Delete";

                pnladJobDetails.Visible = true;
                pnlAdDate.Visible = true;
                pnlAdFunctions.Visible = false;
                pnlAdSkills.Visible = true;
                cmbJobNAme.Visible = true;
                txtJobname.Visible = false;

                ComboBoxHelper.PopulateJobNames(cmbJobNAme);
                rtbJobDescription.Clear();
                rtbJobLocation.Clear();
                calStartDate.SelectionStart = DateTime.Today;
                calEndDate.SelectionStart = DateTime.Today.AddDays(1);
                nudDuration.Value = 8;
                cbJobCompleted.Checked = false;
                sbxPNeeded.Value = 1;

                action = "DELETE";
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }

        }
        private void btnEmpEdit_Click(object sender, EventArgs e)
        {
            //make textboxes editable
            txtEmpFName.Enabled = true;
            txtEmpLName.Enabled = true;
            btnEmpSave.Visible = true;
            btnEmpEdit.Visible = false;


        }

        private void btnEmpSave_Click(object sender, EventArgs e)
        {
            //Update employer details
            if (txtEmpFName.Text == "" || txtEmpLName.Text == "")
            {
                MessageBox.Show("Please enter all the details for the employer!");
                return;
            }
            string empId = txtEmpID.Text;
            string fName = txtEmpFName.Text;
            string lName = txtEmpLName.Text;


            using (SqlConnection conn = new SqlConnection(DatabaseConfig.GetConStr()))
            {
                conn.Open();
                string sql = "UPDATE EMPLOYER SET LastName = @lName, FirstName = @fName WHERE EmployerID = @empId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@lName", lName);
                    cmd.Parameters.AddWithValue("@fName", fName);
                    cmd.Parameters.AddWithValue("@empId", @empId);


                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Employer Updated successfully!");
                        pnlEmpMenu.Visible = true;
                        PnlEmpInfo.Visible = false;
                        pnlEmpDetails.Visible = false;
                        btnEmpEdit.Visible = false;
                        pnlEmpPass.Visible = false;
                        btnBackEmp.Visible = false;
                        btnEmpSave.Visible = false;

                    }


                }
            }
        }

        private void btnEmpSavePass_Click(object sender, EventArgs e)
        {
            //Update employer password
            string empId = txtEmpID.Text;
            string newPass = txtEmpNew.Text;
            string confirmPass = txtEmpConfirm.Text;

            if (newPass != confirmPass)
            {
                MessageBox.Show("passwords do not match!");
                return;
            }
            else if (txtEmpPass.Text.Trim().Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            newPass = PasswordHasser.HashPassword(newPass);
            MessageBox.Show(newPass);

            using (SqlConnection conn = new SqlConnection(DatabaseConfig.GetConStr()))
            {
                conn.Open();
                string sql = "UPDATE EMPLOYER SET Password = @new WHERE EmployerID = @empId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@new", newPass);
                    cmd.Parameters.AddWithValue("@empId", @empId);


                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Password Updated successfully!");

                        pnlEmpPass.Visible = false;
                        txtEmpNew.Clear();
                        txtEmpConfirm.Clear();

                    }


                }
            }
        }

        private void cbxJobName_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlJobDetails.Visible = false;
            int jobID = ComboBoxHelper.GetSelectedID(cbxJobName);
            Job thisJob = JobManager.LoadJob(jobID);

            lblGuardDetails.Text = JobManager.CountPrisonersOnJob(thisJob) + " prisoners are assigned to " + thisJob.JobName + ". Send " + thisJob.AssignedGuards() + " guard(s) to job site";
            pnlGuardDetails.Visible = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            pnlGuardDetails.Visible = false;

            cbxJobName.SelectedIndexChanged -= cbxJobName_SelectedIndexChanged;
            cbxJobName.SelectedIndex = -1;
            cbxJobName.SelectedIndexChanged += cbxJobName_SelectedIndexChanged;

            pnlJobDetails.Visible = true;
        }



        //LOGOUT & BACK BUTTONS

        private void button3_Click(object sender, EventArgs e)
        {
            frmLogIn login = new frmLogIn();
            login.logOut();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 0; ;
        }
        private void btnMEmpBack_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 0; ;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 0; ;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 0; ;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 0; ;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 0; ;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 0; ;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            tcAdmin.SelectedIndex = 0; ;

            lstMPAllSkills.DataSource = null;
            lstMPAllSkills.Items.Clear();
            lstMPCurrentSkills.DataSource = null;
            lstMPCurrentSkills.Items.Clear();

            pnlPInfo.Visible = false;
            pnlMPSkills.Visible = false;
            cmbMPPrisonerNo.Visible = false;
            pnlMPChoices.Visible = true;

            cmbMPPrisonerNo.SelectedIndexChanged -= cmbMPPrisonerNo_SelectedIndexChanged;
            ComboBoxHelper.PopulatePrisonerNumbers(cmbMPPrisonerNo);
            cmbMPPrisonerNo.SelectedIndexChanged += cmbMPPrisonerNo_SelectedIndexChanged;

            isAddingPrisoner = false;
            isDeletingPrisoner = false;
            isupdatingPrisoner = false;
            Ppassword = "";

        }

        //END OF LOGOUT & BACK BUTTONS
        private void btnEmpBack_Click(object sender, EventArgs e)
        {
            pnlEmpMenu.Visible = true;
            PnlEmpInfo.Visible = false;
            pnlEmpDetails.Visible = false;
            btnEmpEdit.Visible = false;
            pnlEmpPass.Visible = false;
            btnBackEmp.Visible = false;
            btnEmpSave.Visible = false;
        }

        private void cbxASJJobName_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (isPopulating || cbxASJJobName.SelectedIndex == -1)
                return;
            ListboxHelper.PopulateCurrentJobSkills(lstASJCurrentSkillOfJob, (int)cbxASJJobName.SelectedValue);
            ListboxHelper.PopulatAvailableJobSkills(lstASJAllSkills, (int)cbxASJJobName.SelectedValue);

        }

        private void btnASJRemove_Click(object sender, EventArgs e)
        {
            JobSkillManager.RemoveJobSkill((int)cbxASJJobName.SelectedValue, ListboxHelper.GetSelectedSkillID(lstASJCurrentSkillOfJob));

            isPopulating = true;
            ListboxHelper.PopulatAvailableJobSkills(lstASJAllSkills, (int)cbxASJJobName.SelectedValue);
            ListboxHelper.PopulateCurrentJobSkills(lstASJCurrentSkillOfJob, (int)cbxASJJobName.SelectedValue);
            isPopulating = false;
        }
        private void btnASJAdd_Click(object sender, EventArgs e)
        {
            JobSkillManager.AddJobSkill((int)cbxASJJobName.SelectedValue, ListboxHelper.GetSelectedSkillID(lstASJAllSkills));

            isPopulating = true;
            ListboxHelper.PopulatAvailableJobSkills(lstASJAllSkills, (int)cbxASJJobName.SelectedValue);
            ListboxHelper.PopulateCurrentJobSkills(lstASJCurrentSkillOfJob, (int)cbxASJJobName.SelectedValue);
            isPopulating = false;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to go back? Some changes will be lost", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                lstMPAllSkills.DataSource = null;
                lstMPAllSkills.Items.Clear();
                lstMPCurrentSkills.DataSource = null;
                lstMPCurrentSkills.Items.Clear();

                pnlPInfo.Visible = false;
                pnlMPSkills.Visible = false;
                cmbMPPrisonerNo.Visible = false;
                pnlMPChoices.Visible = true;

                cmbMPPrisonerNo.SelectedIndexChanged -= cmbMPPrisonerNo_SelectedIndexChanged;
                ComboBoxHelper.PopulatePrisonerNumbers(cmbMPPrisonerNo);
                cmbMPPrisonerNo.SelectedIndexChanged += cmbMPPrisonerNo_SelectedIndexChanged;

                isAddingPrisoner = false;
                isDeletingPrisoner = false;
                isupdatingPrisoner = false;
                Ppassword = "";
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            string skill = "";

            skill = lstMJAllSkills.SelectedItem.ToString();
            lstMJCurrentSkills.Items.Add(skill);
            lstMJAllSkills.Items.RemoveAt(lstMJAllSkills.SelectedIndex);
            //JobSkillManager.AddJobSkill(JobID, skill);

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string skill = "";

            skill = lstMJCurrentSkills.SelectedItem.ToString();
            lstMJAllSkills.Items.Add(skill);
            lstMJCurrentSkills.Items.RemoveAt(lstMJCurrentSkills.SelectedIndex);
            //JobSkillManager.RemoveJobSkill(JobID,skill);
        }

        private void frmAdmin_Load(object sender, EventArgs e)
        {
            pnladJobDetails.Visible = false;
            pnlAdDate.Visible = false;
            pnlAdSkills.Visible = false;
        }

        private void imgMEPass_MouseDown(object sender, MouseEventArgs e)
        {
            txtEmpPass.UseSystemPasswordChar = false;
            imgMEPass.Image = Properties.Resources.hide;
        }

        private void imgMEPass_MouseUp(object sender, MouseEventArgs e)
        {
            txtEmpPass.UseSystemPasswordChar = true;
            imgMEPass.Image = Properties.Resources.view;
        }

        private void imgFMENewshow_MouseDown(object sender, MouseEventArgs e)
        {
            txtEmpNew.UseSystemPasswordChar = false;
            imgFMENewshow.Image = Properties.Resources.hide;
        }

        private void imgFMENewshow_MouseUp(object sender, MouseEventArgs e)
        {
            txtEmpNew.UseSystemPasswordChar = true;
            imgFMENewshow.Image = Properties.Resources.view;
        }

        private void imgMPpass_MouseDown(object sender, MouseEventArgs e)
        {
            txtNewPassword.UseSystemPasswordChar = false;
            imgMPpass.Image = Properties.Resources.hide;
        }

        private void imgMPpass_MouseUp(object sender, MouseEventArgs e)
        {
            txtNewPassword.UseSystemPasswordChar = true;
            imgMPpass.Image = Properties.Resources.view;
        }

        private void cmbJobNAme_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbJobNAme.SelectedIndex != -1 && (action == "EDIT" || action == "DELETE"))
                {
                    selectedJobID = ComboBoxHelper.GetSelectedID(cmbJobNAme);

                    if (selectedJobID > 0)
                    {
                        // Load job details using JobManager
                        selectedJob = JobManager.LoadJob(selectedJobID);

                        if (selectedJob != null)
                        {                            
                            txtJobname.Text = selectedJob.JobName;
                            rtbJobDescription.Text = selectedJob.JobDescription;
                            rtbJobLocation.Text = selectedJob.JobLocation;
                            calStartDate.SelectionStart = selectedJob.StartDate;
                            calEndDate.SelectionStart = selectedJob.EndDate;
                            nudDuration.Value = selectedJob.DurationPerDay;
                            cbJobCompleted.Checked = selectedJob.Completed;
                            sbxPNeeded.Value = selectedJob.PrisonersNeeded;

                            // Populate employer name from EmployerID

                            cbxMJEmpName.Enabled = true;
                            string employerName = GetEmployerNameById(selectedJob.EmployerID);
                            if (!string.IsNullOrEmpty(employerName))
                            {
                                cbxMJEmpName.SelectedIndexChanged -= cbxMJEmpName_SelectedIndexChanged;
                                cbxMJEmpName.DataSource = null;
                                cbxMJEmpName.Items.Clear();
                                cbxMJEmpName.Items.Add(employerName);
                                cbxMJEmpName.SelectedIndex = 0;
                                cbxMJEmpName.Enabled = false;
                                cbxMJEmpName.SelectedIndexChanged += cbxMJEmpName_SelectedIndexChanged;
                            }

                            if (action == "DELETE")
                            {
                                sbxPNeeded.Enabled = false;
                                pnlAdDate.Enabled = false;
                                ListboxHelper.PopulateCurrentJobSkills(lstMJCurrentSkills, selectedJobID);
                                ListboxHelper.PopulateAvailablePSkills(lstMJAllSkills, selectedJobID);
                                pnlAdSkills.Enabled = false;
                                txtJobname.ReadOnly = true;
                                rtbJobDescription.ReadOnly = true;
                                rtbJobLocation.ReadOnly = true;
                                Job newJob = new Job();
                                
                            }
                            else if (action == "EDIT")
                            {
                                ListboxHelper.PopulateCurrentJobSkills(lstMJCurrentSkills, selectedJobID);
                                ListboxHelper.PopulateAvailablePSkills(lstMJAllSkills, selectedJobID);
                                txtJobname.ReadOnly = false;
                                rtbJobDescription.ReadOnly = false;
                                rtbJobLocation.ReadOnly = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading job details: " + ex.Message);
            }
        }

        private void txtJobname_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmAdmin_Load_1(object sender, EventArgs e)
        {
            tcAdmin.Appearance = TabAppearance.FlatButtons;
            tcAdmin.ItemSize = new Size(0, 1);
            tcAdmin.SizeMode = TabSizeMode.Fixed;
        }

        private void pnlAdFunctions_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbxMJEmpName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnASJAdd_Click_1(object sender, EventArgs e)
        {
            JobSkillManager.AddJobSkill((int)cbxASJJobName.SelectedValue, ListboxHelper.GetSelectedSkillID(lstASJAllSkills));

            isPopulating = true;
            ListboxHelper.PopulatAvailableJobSkills(lstASJAllSkills, (int)cbxASJJobName.SelectedValue);
            ListboxHelper.PopulateCurrentJobSkills(lstASJCurrentSkillOfJob, (int)cbxASJJobName.SelectedValue);
            isPopulating = false;
        }

        private void btnASJRemove_Click_1(object sender, EventArgs e)
        {
            JobSkillManager.RemoveJobSkill((int)cbxASJJobName.SelectedValue, ListboxHelper.GetSelectedSkillID(lstASJCurrentSkillOfJob));

            isPopulating = true;
            ListboxHelper.PopulatAvailableJobSkills(lstASJAllSkills, (int)cbxASJJobName.SelectedValue);
            ListboxHelper.PopulateCurrentJobSkills(lstASJCurrentSkillOfJob, (int)cbxASJJobName.SelectedValue);
            isPopulating = false;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            JobSkillManager.AddJobSkill(selectedJobID, ListboxHelper.GetSelectedSkillID(lstMJAllSkills));

            isPopulating = true;
            ListboxHelper.PopulatAvailableJobSkills(lstMJAllSkills, selectedJobID);
            ListboxHelper.PopulateCurrentJobSkills(lstMJCurrentSkills, selectedJobID);
            isPopulating = false;
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            JobSkillManager.RemoveJobSkill(selectedJobID, ListboxHelper.GetSelectedSkillID(lstMJCurrentSkills));

            isPopulating = true;
            ListboxHelper.PopulatAvailableJobSkills(lstMJAllSkills, selectedJobID);
            ListboxHelper.PopulateCurrentJobSkills(lstMJCurrentSkills, selectedJobID);
            isPopulating = false;
        }
    }
} 

            


    
            

     


       
    

    

