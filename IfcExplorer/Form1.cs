using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace IfcExplorer
{


    public partial class Form1 : Form
    {
        /*Variables générales*/
        XDocument xml;
        List<ListBox> listbox = new List<ListBox>();
        List<string> inheritance = new List<string>();
        List<string> path = new List<string>();
        int[] activei = { -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        /*Fonctions générales*/
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            listbox.Add(listBox1);listbox.Add(listBox2);listbox.Add(listBox3);
            listbox.Add(listBox4);listbox.Add(listBox5);listbox.Add(listBox6);
            listbox.Add(listBox7);listbox.Add(listBox8);listbox.Add(listBox9);
        }
        private void Inheritance(int current_listbox)
        {

            for (int i = 1; i < current_listbox+1; ++i)
            {
                string name = listbox[i - 1].SelectedItem.ToString();

                IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                                  where (string)ifcclass.Attribute("name") == name
                                                  select ifcclass;

                if (selection != null)
                {
                    if (selection.Elements("direct_att").Count() > 0 || selection.Elements("inverse_att").Count() > 0) { inheritance.Add("\r\n↓ Inherited from " + selection.ElementAt(0).Attribute("name").Value + " ↓\r\n"); }

                    if (selection.Elements("direct_att").Count() > 0)
                    {
                        foreach (XElement element in selection.Elements("direct_att")) { inheritance.Add("+ " + element.Attribute("name").Value + " / " + element.Attribute("type").Value); }
                    }
                    if (selection.Elements("inverse_att").Count() > 0)
                    {
                        foreach (XElement element in selection.Elements("inverse_att")) { inheritance.Add("- " + element.Attribute("name").Value + " / " + element.Attribute("type").Value); }
                    }
                }
            }
        }
        private void SearchTypeTag(string type, string tag)
        {
            IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                              where (string)ifcclass.Attribute("type").Value == type
                                              select ifcclass;

            List<XElement> classes = selection.ToList();

            foreach (XElement classe in classes)
            {
                List<XElement> liste = classe.Elements(tag).ToList();
                foreach (XElement element in liste)
                {
                    if (textBox1.Text.Replace(" ", "") == "*" || element.Attribute("name").Value.ToLower().Contains(textBox1.Text.ToLower()))
                    {
                        listBox10.Items.Add(classe.Attribute("name").Value + " (" + element.Attribute("name").Value + ")");
                    }
                }
            }
        
        }
        private void SearchNaturalName(string type, string code)
        {
            IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                              where (int)ifcclass.Elements(code).Count() > 0 && (string)ifcclass.Attribute("type") == type
                                              select ifcclass;

            List<XElement> list = selection.ToList();

            foreach (XElement element in list)
            { 
                if (textBox1.Text.Replace(" ", "") == "*" || element.Element(code).Value.ToLower().Contains(textBox1.Text.ToLower()))
                {
                    Console.WriteLine(element.Element(code).Value.ToLower());
                    listBox10.Items.Add(element.Attribute("name").Value + " (" + element.Element(code).Value + ")");
                }
            }
        }
        private void SearchIfcName(string type)
        {
            IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                              where (string)ifcclass.Attribute("type") == type
                                              select ifcclass;

            List<XElement> list = selection.ToList();

            foreach (XElement element in list) 
            {
                if (textBox1.Text.Replace(" ", "") == "*" || element.Attribute("name").Value.ToLower().Contains(textBox1.Text.ToLower()))
                listBox10.Items.Add(element.Attribute("name").Value);
                Console.WriteLine(element.Attribute("name").Value);
            }
        }


        /*Fonction(s) sur detail(s)*/
        private void DetailsGeneralInformations(int current_listbox)
        {
            string name = listbox[current_listbox - 1].SelectedItem.ToString();

            IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                        where (string)ifcclass.Attribute("name") == name
                                        select ifcclass;

            if (selection != null)
            {
                    string line1 = "# GENERAL INFORMATIONS\r\n";
                    string line2 = "\r\n";
                    string line3 = "Class name : " + selection.ElementAt(0).Attribute("name").Value + "\r\n";
                    string line4 = "Class type : " + selection.ElementAt(0).Attribute("type").Value + "\r\n";
                    textBox2.Text = textBox2.Text + line1 + line2 + line3 + line4;
            }
        }
        private void DetailsNaturalNames(int current_listbox)
        {
            string name = listbox[current_listbox - 1].SelectedItem.ToString();

            IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                              where (string)ifcclass.Attribute("name") == name
                                              select ifcclass;

            if (selection != null)
            {
                string line1 = "__________________________________________________\r\n";
                string line2 = "\r\n";
                string line3 = "# NATURAL LANGUAGE NAME(S)\r\n";
                string line4 = "\r\n";
                textBox2.Text = textBox2.Text + line1 + line2 + line3 + line4;

                if (selection.Elements("en").Count() > 0) { textBox2.Text = textBox2.Text + "EN : " + selection.Elements("en").ElementAt(0).Value + "\r\n"; }
                if (selection.Elements("fr").Count() > 0) { textBox2.Text = textBox2.Text + "FR : " + selection.Elements("fr").ElementAt(0).Value + "\r\n"; }
                if (selection.Elements("de").Count() > 0) { textBox2.Text = textBox2.Text + "DE : " + selection.Elements("de").ElementAt(0).Value + "\r\n"; }
            }
        }
        private void DetailsAttributes(int current_listbox)
        {
            string line1 = "__________________________________________________\r\n";
            string line2 = "\r\n";
            string line3 = "# DIRECT (+) AND INVERSE ATTRIBUTE(S) (-)\r\n";
            textBox2.Text = textBox2.Text + line1 + line2 + line3;

            foreach (string element in inheritance)
            {
                textBox2.Text = textBox2.Text + element + "\r\n";
            }
        }

        /*Fonction(s) sur liste(s)*/
        private void NextLevel(int current_listbox)
        {
            List<int> index = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            string name = listbox[current_listbox - 1].SelectedItem.ToString();

            IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                              where (string)ifcclass.Attribute("name") == name
                                              select ifcclass;

            List<XElement> list = selection.Elements("sub").ToList();

            foreach (int listbox_number in index) { if (listbox_number > current_listbox) { listbox[listbox_number - 1].Items.Clear(); } }
            foreach (XElement element in list) { listbox[current_listbox].Items.Add(element.Attribute("name").Value); }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && radioButton1.Checked == true)
            {
                NextLevel(1);
                button3.Enabled = true;

                /*Details update*/
                textBox2.Text = "";
                DetailsGeneralInformations(1);
                DetailsNaturalNames(1);
                inheritance.Clear(); Inheritance(1);
                DetailsAttributes(1);
            }

            if (listBox1.SelectedItem != null && radioButton2.Checked == true)
            {
                List<int> index = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                string name = listbox[0].SelectedItem.ToString();

                IEnumerable<XElement> selection = from ifcgroup in xml.Elements("list").Elements("group")
                                                  where (string)ifcgroup.Attribute("name") == name
                                                  select ifcgroup;

                List<XElement> list = selection.Elements("res").ToList();

                foreach (int listbox_number in index) { if (listbox_number > 1) { listbox[listbox_number - 1].Items.Clear(); } }
                foreach (XElement element in list) { listbox[1].Items.Add(element.Attribute("name").Value); }

                button3.Enabled = true;

                /*Details update*/
                textBox2.Text = "";
                //inheritance.Clear(); Inheritance(1);
            }
        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            { 
                NextLevel(2);

                /*Details update*/
                textBox2.Text = "";
                DetailsGeneralInformations(2);
                DetailsNaturalNames(2);
                inheritance.Clear(); Inheritance(2);
                DetailsAttributes(2);
            }

        }
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox3.SelectedItem != null)
            {
                NextLevel(3);

                /*Details update*/
                textBox2.Text = "";
                DetailsGeneralInformations(3);
                DetailsNaturalNames(3);
                inheritance.Clear(); Inheritance(3);
                DetailsAttributes(3);
            }
        }
        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox4.SelectedItem != null)
            {
                NextLevel(4);

                /*Details update*/
                textBox2.Text = "";
                DetailsGeneralInformations(4);
                DetailsNaturalNames(4);
                inheritance.Clear(); Inheritance(4);
                DetailsAttributes(4);
            }
        }
        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox5.SelectedItem != null)
            {
                NextLevel(5);

                /*Details update*/
                textBox2.Text = "";
                DetailsGeneralInformations(5);
                DetailsNaturalNames(5);
                inheritance.Clear(); Inheritance(5);
                DetailsAttributes(5);
            }
        }
        private void listBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox6.SelectedItem != null)
            {
                NextLevel(6);

                /*Details update*/
                textBox2.Text = "";
                DetailsGeneralInformations(6);
                DetailsNaturalNames(6);
                inheritance.Clear(); Inheritance(6);
                DetailsAttributes(6);
            }
        }
        private void listBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox7.SelectedItem != null)
            {
                NextLevel(7);

                /*Details update*/
                textBox2.Text = "";
                DetailsGeneralInformations(7);
                DetailsNaturalNames(7);
                inheritance.Clear(); Inheritance(7);
                DetailsAttributes(7);
            }
 
        }
        private void listBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox8.SelectedItem != null)
            {
                NextLevel(8);

                /*Details update*/
                textBox2.Text = "";
                DetailsGeneralInformations(8);
                DetailsNaturalNames(8);
                inheritance.Clear(); Inheritance(8);
                DetailsAttributes(8);
            }
        }
        private void listBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox9.SelectedItem != null)
            {
                /*Details update*/

                textBox2.Text = "";
                DetailsGeneralInformations(9);
                DetailsNaturalNames(9);
                inheritance.Clear(); Inheritance(9);
                DetailsAttributes(9);
            }
        }

        /*Fonction(s) sur liste(s) déroulante(s)*/
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = true; comboBox2.Items.Clear(); comboBox2.ResetText(); comboBox2.Text = "click to select...";
            textBox1.Text = ""; textBox1.Enabled = false; button1.Enabled = false;


            if (comboBox1.SelectedIndex == 0)
            {
                comboBox2.Items.Add("Name (IFC)");
                comboBox2.Items.Add("Name (English)");
                comboBox2.Items.Add("Name (French)");
                comboBox2.Items.Add("Name (German)");
            }
            if (comboBox1.SelectedIndex == 1)
            {
                comboBox2.Items.Add("Name (IFC)");
                comboBox2.Items.Add("Name (English)");
                comboBox2.Items.Add("Name (French)");
                comboBox2.Items.Add("Name (German)");
            }
            if (comboBox1.SelectedIndex == 2)
            {
                comboBox2.Items.Add("Name (IFC)");
                comboBox2.Items.Add("Name (English)");
                comboBox2.Items.Add("Name (French)");
                comboBox2.Items.Add("Name (German)");
                comboBox2.Items.Add("Direct attribute (+)");
                comboBox2.Items.Add("Inverse attribute (-)");
                comboBox2.Items.Add("Subclass");
                comboBox2.Items.Add("Superclass");
            }
            if (comboBox1.SelectedIndex == 3)
            {
                comboBox2.Items.Add("Name (IFC)");
                comboBox2.Items.Add("Name (English)");
                comboBox2.Items.Add("Name (French)");
                comboBox2.Items.Add("Name (German)");
                comboBox2.Items.Add("Enumerated type constant");
            }
            if (comboBox1.SelectedIndex == 4)
            {
                comboBox2.Items.Add("Name (IFC)");
                comboBox2.Items.Add("Name (English)");
                comboBox2.Items.Add("Name (French)");
                comboBox2.Items.Add("Name (German)");
                comboBox2.Items.Add("Enumerated property constant");
            }
            if (comboBox1.SelectedIndex == 5)
            {
                comboBox2.Items.Add("Name (IFC)");
                comboBox2.Items.Add("Name (English)");
                comboBox2.Items.Add("Name (French)");
                comboBox2.Items.Add("Name (German)");
                comboBox2.Items.Add("Property name");
                comboBox2.Items.Add("Related class name");
            }
            if (comboBox1.SelectedIndex == 6)
            {
                comboBox2.Items.Add("Name (IFC)");
                comboBox2.Items.Add("Name (English)");
                comboBox2.Items.Add("Name (French)");
                comboBox2.Items.Add("Name (German)");
                comboBox2.Items.Add("Quantity name");
                comboBox2.Items.Add("Related class name");
            }
            if (comboBox1.SelectedIndex == 7)
            {
                comboBox2.Items.Add("Name (IFC)");
                comboBox2.Items.Add("Name (English)");
                comboBox2.Items.Add("Name (French)");
                comboBox2.Items.Add("Name (German)");
                comboBox2.Items.Add("Selectable type");
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = true;
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == 0) { xml = XDocument.Load(System.IO.Directory.GetCurrentDirectory() + @"\4_0_ADD2_TC1.xml"); }
            if (comboBox3.SelectedIndex == 1) { xml = XDocument.Load(System.IO.Directory.GetCurrentDirectory() + @"\4_3_RC2.xml"); }
            if (comboBox3.SelectedIndex == 2) { xml = XDocument.Load(System.IO.Directory.GetCurrentDirectory() + @"\4_3_RC4.xml"); }

            comboBox1.Text = "click to select..."; comboBox1.Enabled = true; listBox1.Enabled = true; comboBox2.Items.Clear(); comboBox2.Text = ""; comboBox2.Enabled = false; radioButton1.Enabled = true;
            radioButton2.Enabled = true; textBox1.Clear(); textBox1.Enabled = false; listBox10.Items.Clear(); listBox10.Refresh(); button3.Enabled = false;

            for (int i = 0; i <= 8; ++i) { listbox[i].Items.Clear(); listbox[i].Refresh(); }
            if (radioButton1.Checked == true)
            {
                listBox1.Items.Add("IfcRoot");
            }
            else
            {
                listBox1.Items.Add("IfcActorResource");
                listBox1.Items.Add("IfcApprovalResource");
                listBox1.Items.Add("IfcConstraintResource");
                listBox1.Items.Add("IfcCostResource");
                listBox1.Items.Add("IfcDateTimeResource");
                listBox1.Items.Add("IfcExternalReferenceResource");
                listBox1.Items.Add("IfcGeometricConstraintResource");
                listBox1.Items.Add("IfcGeometricModelResource");
                listBox1.Items.Add("IfcGeometryResource");
                listBox1.Items.Add("IfcMaterialResource");
                listBox1.Items.Add("IfcMeasureResource");
                listBox1.Items.Add("IfcPresentationAppearanceResource");
                listBox1.Items.Add("IfcPresentationDefinitionResource");
                listBox1.Items.Add("IfcPresentationOrganizationResource");
                listBox1.Items.Add("IfcProfileResource");
                listBox1.Items.Add("IfcPropertyResource");
                listBox1.Items.Add("IfcQuantityResource");
                listBox1.Items.Add("IfcRepresentationResource");
                listBox1.Items.Add("IfcStructuralLoadResource");
                listBox1.Items.Add("IfcTopologyResource");
                listBox1.Items.Add("IfcUtilityResource");
            }


        }

        /*Fonction(s) sur zone(s) de texte*/
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Replace(" ", "") != "") { button1.Enabled = true; } else { button1.Enabled = false; }
        }

        /*Fonction(s) sur bouton(s)*/
        private void button1_Click(object sender, EventArgs e)
        {
            /*Effacer les résultats précédents*/
            listBox10.Items.Clear();

            /*0,0,"*"*/
            if (comboBox1.SelectedIndex == 0 && comboBox2.SelectedIndex == 0 && textBox1.Text.Replace(" ","") =="*")
            {
                IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                                  select ifcclass;

                List<XElement> list = selection.ToList();

                foreach (XElement element in list) { listBox10.Items.Add(element.Attribute("name").Value); }
            }
            /*0,0*/
            if (comboBox1.SelectedIndex == 0 && comboBox2.SelectedIndex == 0)
            {
                IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                                  where ifcclass.Attribute("name").Value.ToLower().Contains(textBox1.Text.ToLower()) == true
                                                  select ifcclass;

                List<XElement> list = selection.ToList();

                foreach (XElement element in list) { listBox10.Items.Add(element.Attribute("name").Value); }
            }
            /*0,1,"*"*/
            if (comboBox1.SelectedIndex == 0 && comboBox2.SelectedIndex == 1 && textBox1.Text.Replace(" ", "") == "*")
            {
                IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                                  where (int)ifcclass.Elements("en").Count() > 0
                                                  select ifcclass;

                List<XElement> list = selection.ToList();

                foreach (XElement element in list) { listBox10.Items.Add(element.Attribute("name").Value + " (" + element.Element("en").Value + ")"); }
            }
            /*0,1*/
            if (comboBox1.SelectedIndex == 0 && comboBox2.SelectedIndex == 1)
            {
                IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                                  where (int)ifcclass.Elements("en").Count() > 0
                                                  select ifcclass;

                List<XElement> list = selection.ToList();

                foreach (XElement element in list)
                {
                    if (element.Element("en").Value.ToLower().Contains(textBox1.Text.ToLower()))
                    {
                        listBox10.Items.Add(element.Attribute("name").Value + " (" + element.Element("en").Value + ")");
                    }
                }
            }
            /*0,2,"*"*/
            if (comboBox1.SelectedIndex == 0 && comboBox2.SelectedIndex == 2 && textBox1.Text.Replace(" ", "") == "*")
            {
                IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                                  where (int)ifcclass.Elements("fr").Count() > 0
                                                  select ifcclass;

                List<XElement> list = selection.ToList();

                foreach (XElement element in list) { listBox10.Items.Add(element.Attribute("name").Value + " (" + element.Element("fr").Value + ")"); }
            }
            /*0,2*/
            if (comboBox1.SelectedIndex == 0 && comboBox2.SelectedIndex == 2)
            {
                IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                                  where (int)ifcclass.Elements("fr").Count() > 0
                                                  select ifcclass;

                List<XElement> list = selection.ToList();

                foreach (XElement element in list) 
                {
                    if (element.Element("fr").Value.ToLower().Contains(textBox1.Text.ToLower()))
                    {
                        listBox10.Items.Add(element.Attribute("name").Value + " (" + element.Element("fr").Value + ")");
                    }
                }
            }
            /*0,3,"*"*/
            if (comboBox1.SelectedIndex == 0 && comboBox2.SelectedIndex == 3 && textBox1.Text.Replace(" ", "") == "*")
            {
                IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                                  where (int)ifcclass.Elements("de").Count() > 0
                                                  select ifcclass;

                List<XElement> list = selection.ToList();

                foreach (XElement element in list) { listBox10.Items.Add(element.Attribute("name").Value + " (" + element.Element("de").Value + ")"); }
            }
            /*0,3*/
            if (comboBox1.SelectedIndex == 0 && comboBox2.SelectedIndex == 3)
            {
                IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                                  where (int)ifcclass.Elements("de").Count() > 0
                                                  select ifcclass;

                List<XElement> list = selection.ToList();

                foreach (XElement element in list)
                {
                    if (element.Element("de").Value.ToLower().Contains(textBox1.Text.ToLower()))
                    {
                        listBox10.Items.Add(element.Attribute("name").Value + " (" + element.Element("de").Value + ")");
                    }
                }
            }


            /*1,0*/
            if (comboBox1.SelectedIndex == 1 && comboBox2.SelectedIndex == 0) { SearchIfcName("Defined type"); }
            /*1,1*/
            if (comboBox1.SelectedIndex == 1 && comboBox2.SelectedIndex == 1) { SearchNaturalName("Defined type", "en"); }
            /*1,2*/
            if (comboBox1.SelectedIndex == 1 && comboBox2.SelectedIndex == 2) { SearchNaturalName("Defined type", "fr"); }
            /*1,3*/
            if (comboBox1.SelectedIndex == 1 && comboBox2.SelectedIndex == 3) { SearchNaturalName("Defined type", "de"); }


            /*2,0*/
            if (comboBox1.SelectedIndex == 2 && comboBox2.SelectedIndex == 0) { SearchIfcName("Entity"); }
            /*2,1*/
            if (comboBox1.SelectedIndex == 2 && comboBox2.SelectedIndex == 1) { SearchNaturalName("Entity", "en"); }
            /*2,2*/
            if (comboBox1.SelectedIndex == 2 && comboBox2.SelectedIndex == 2) { SearchNaturalName("Entity", "fr"); }
            /*2,3*/
            if (comboBox1.SelectedIndex == 2 && comboBox2.SelectedIndex == 3) { SearchNaturalName("Entity", "de"); }
            /*2,4,*/
            if (comboBox1.SelectedIndex == 2 && comboBox2.SelectedIndex == 4) { SearchTypeTag("Entity", "direct_att"); }
            /*2,5,*/
            if (comboBox1.SelectedIndex == 2 && comboBox2.SelectedIndex == 5) { SearchTypeTag("Entity", "inverse_att"); }
            /*2,6,*/
            if (comboBox1.SelectedIndex == 2 && comboBox2.SelectedIndex == 6) { SearchTypeTag("Entity", "sub"); }
            /*2,7,*/
            if (comboBox1.SelectedIndex == 2 && comboBox2.SelectedIndex == 7) { SearchTypeTag("Entity", "sup"); }


            /*3,0*/
            if (comboBox1.SelectedIndex == 3 && comboBox2.SelectedIndex == 0) { SearchIfcName("Type enumeration"); }
            /*3,1*/
            if (comboBox1.SelectedIndex == 3 && comboBox2.SelectedIndex == 1) { SearchNaturalName("Type enumeration", "en"); }
            /*3,2*/
            if (comboBox1.SelectedIndex == 3 && comboBox2.SelectedIndex == 2) { SearchNaturalName("Type enumeration", "fr"); }
            /*3,3*/
            if (comboBox1.SelectedIndex == 3 && comboBox2.SelectedIndex == 3) { SearchNaturalName("Type enumeration", "de"); }
            /*3,4,*/
            if (comboBox1.SelectedIndex == 3 && comboBox2.SelectedIndex == 4) { SearchTypeTag("Type enumeration", "constant"); }


            /*4,0*/
            if (comboBox1.SelectedIndex == 4 && comboBox2.SelectedIndex == 0) { SearchIfcName("Property enumeration"); }
            /*4,1*/
            if (comboBox1.SelectedIndex == 4 && comboBox2.SelectedIndex == 1) { SearchNaturalName("Property enumeration", "en"); }
            /*4,2*/
            if (comboBox1.SelectedIndex == 4 && comboBox2.SelectedIndex == 2) { SearchNaturalName("Property enumeration", "fr"); }
            /*4,3*/
            if (comboBox1.SelectedIndex == 4 && comboBox2.SelectedIndex == 3) { SearchNaturalName("Property enumeration", "de"); }
            /*4,4,*/
            if (comboBox1.SelectedIndex == 4 && comboBox2.SelectedIndex == 4) { SearchTypeTag("Property enumeration", "constant"); }

            /*5,0*/
            if (comboBox1.SelectedIndex == 5 && comboBox2.SelectedIndex == 0) { SearchIfcName("Property set"); }  
            /*5,1*/
            if (comboBox1.SelectedIndex == 5 && comboBox2.SelectedIndex == 1) { SearchNaturalName("Property set", "en"); }
            /*5,2*/
            if (comboBox1.SelectedIndex == 5 && comboBox2.SelectedIndex == 2) { SearchNaturalName("Property set", "fr"); }
            /*5,3*/
            if (comboBox1.SelectedIndex == 5 && comboBox2.SelectedIndex == 3) { SearchNaturalName("Property set", "de"); }
            /*5,4*/
            if (comboBox1.SelectedIndex == 5 && comboBox2.SelectedIndex == 4) { SearchTypeTag("Property set", "property"); }
            /*5,5*/
            if (comboBox1.SelectedIndex == 5 && comboBox2.SelectedIndex == 5) { SearchTypeTag("Property set", "relatedclass"); }

            /*6,0*/
            if (comboBox1.SelectedIndex == 6 && comboBox2.SelectedIndex == 0) { SearchIfcName("Quantity set"); }
            /*6,1*/
            if (comboBox1.SelectedIndex == 6 && comboBox2.SelectedIndex == 1) { SearchNaturalName("Quantity set", "en"); }
            /*6,2*/
            if (comboBox1.SelectedIndex == 6 && comboBox2.SelectedIndex == 2) { SearchNaturalName("Quantity set", "fr"); }
            /*6,3*/
            if (comboBox1.SelectedIndex == 6 && comboBox2.SelectedIndex == 3) { SearchNaturalName("Quantity set", "de"); }
            /*6,4*/
            if (comboBox1.SelectedIndex == 6 && comboBox2.SelectedIndex == 4) { SearchTypeTag("Quantity set", "quantity"); }
            /*6,5*/
            if (comboBox1.SelectedIndex == 6 && comboBox2.SelectedIndex == 5) { SearchTypeTag("Quantity set", "relatedclass"); }

            /*7,0*/
            if (comboBox1.SelectedIndex == 7 && comboBox2.SelectedIndex == 0) { SearchIfcName("Type select"); }
            /*7,1*/
            if (comboBox1.SelectedIndex == 7 && comboBox2.SelectedIndex == 1) { SearchNaturalName("Type select", "en"); }
            /*7,2*/
            if (comboBox1.SelectedIndex == 7 && comboBox2.SelectedIndex == 2) { SearchNaturalName("Type select", "fr"); }
            /*7,3*/
            if (comboBox1.SelectedIndex == 7 && comboBox2.SelectedIndex == 3) { SearchNaturalName("Type select", "de"); }
            /*7,4*/
            if (comboBox1.SelectedIndex == 7 && comboBox2.SelectedIndex == 4) { SearchTypeTag("Type select", "type"); }

            /*Compter les résultats*/
            label1.Text = listBox10.Items.Count.ToString();

            /*Sélectionner le premier élément*/
            if (listBox10.Items.Count > 0) { listBox10.SelectedIndex = 0; }

        }
        private void button3_Click(object sender, EventArgs e)
        {
            List<string> graphml = new List<string>();

            /*Head*/
            graphml.Add("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>");
            graphml.Add("<graphml xmlns=\"http://graphml.graphdrawing.org/xmlns\" xmlns:java=\"http://www.yworks.com/xml/yfiles-common/1.0/java\" xmlns:sys=\"http://www.yworks.com/xml/yfiles-common/markup/primitives/2.0\" xmlns:x=\"http://www.yworks.com/xml/yfiles-common/markup/2.0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:y=\"http://www.yworks.com/xml/graphml\" xmlns:yed=\"http://www.yworks.com/xml/yed/3\" xsi:schemaLocation=\"http://graphml.graphdrawing.org/xmlns http://www.yworks.com/xml/schema/graphml/1.1/ygraphml.xsd\" >");
            graphml.Add("  <!--Created by IfcExplorer-->");
            graphml.Add("  <key attr.name=\"Description\" attr.type=\"string\" for=\"graph\" id =\"d0\" />");
            graphml.Add("  <key for=\"port\" id=\"d1\" yfiles.type=\"portgraphics\"/>");
            graphml.Add("  <key for=\"port\" id=\"d2\" yfiles.type=\"portgeometry\"/>");
            graphml.Add("  <key for=\"port\" id=\"d3\" yfiles.type=\"portuserdata\"/>");
            graphml.Add("  <key attr.name=\"url\" attr.type=\"string\" for=\"node\" id=\"d4\"/>");
            graphml.Add("  <key attr.name=\"description\" attr.type=\"string\" for=\"node\" id =\"d5\"/>");
            graphml.Add("  <key for=\"node\" id=\"d6\" yfiles.type=\"nodegraphics\"/>");
            graphml.Add("  <key for=\"graphml\" id=\"d7\" yfiles.type=\"resources\"/>");
            graphml.Add("  <key attr.name=\"url\" attr.type=\"string\" for=\"edge\" id=\"d8\"/>");
            graphml.Add("  <key attr.name=\"description\" attr.type=\"string\" for=\"edge\" id=\"d9\"/>");
            graphml.Add("  <key for=\"edge\" id=\"d10\" yfiles.type=\"edgegraphics\"/>");
            graphml.Add("  <graph edgedefault=\"directed\" id=\"G\">");
            graphml.Add("    <data key=\"d0\"/>");

            /*Body*/
            blueNode(graphml, listbox[0].SelectedItem.ToString(), 0, 0);

            for (int i = 1; i< 9; ++i)
            {               
                if (listbox[i].Items.Count > 0)
                {
                    for (int j = 0; j < listbox[i].Items.Count; ++j)
                    {
                        if (listbox[i].SelectedIndex == j)
                        {
                            blueNode(graphml, listbox[i].Items[j].ToString(), j*150, i*150);
                            edge(graphml, listbox[i - 1].SelectedItem.ToString(), listbox[i].Items[j].ToString());
                        }
                        else
                        {
                            whiteNode(graphml, listbox[i].Items[j].ToString(), j*150, i*150);
                            edge(graphml, listbox[i - 1].SelectedItem.ToString(), listbox[i].Items[j].ToString());
                        }
                    }
                }
            }

            /*Tail*/
            graphml.Add("  </graph>");
            graphml.Add("  <data key=\"d7\">");
            graphml.Add("    <y:Resources/>");
            graphml.Add("  </data>");
            graphml.Add("</graphml>");

            /*Enregistrement*/
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "graphml files (*.graphml)|*.graphml";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string name = saveFileDialog1.FileName;
                StreamWriter writer = File.CreateText(name);
                foreach (string element in graphml) { writer.WriteLine(element); }
                writer.Close();
            }
        }

        /*Fonctions pour graphML*/
        private void blueNode(List<string> list, string ifc, int x, int y) 
        {
            list.Add("    <node id=\""+ ifc +"\">");
            list.Add("      <data key=\"d5\"/>");
            list.Add("      <data key=\"d6\">");
            list.Add("        <y:ShapeNode>");
            list.Add("          <y:Geometry height=\"50.0\" width=\"100.0\" x=\"" + x.ToString() + ".0\" y=\"" + y.ToString() + ".0\"/>");
            list.Add("          <y:Fill color=\"#3366FF\" transparent =\"false\"/>");
            list.Add("          <y:BorderStyle color=\"#000000\" raised=\"false\" type =\"line\" width=\"1.0\"/>");
            list.Add("          <y:NodeLabel alignment=\"center\" autoSizePolicy=\"content\" fontFamily=\"Dialog\" fontSize=\"10\" fontStyle=\"plain\" hasBackgroundColor=\"false\" hasLineColor=\"false\" height=\"18.701171875\" horizontalTextPosition=\"center\" iconTextGap=\"4\" modelName=\"custom\" textColor=\"#FFFFFF\" verticalTextPosition=\"bottom\" visible=\"true\" width=\"40.03515625\" x=\"29.982421875\" xml:space=\"preserve\" y=\"15.6494140625\">"+ ifc +"<y:LabelModel><y:SmartNodeLabelModel distance=\"4.0\"/></y:LabelModel><y:ModelParameter><y:SmartNodeLabelModelParameter labelRatioX=\"0.0\" labelRatioY =\"0.0\" nodeRatioX =\"0.0\" nodeRatioY =\"0.0\" offsetX =\"0.0\" offsetY =\"0.0\" upX =\"0.0\" upY =\" - 1.0\" /></y:ModelParameter></y:NodeLabel>");
            list.Add("          <y:Shape type=\"rectangle\"/>");
            list.Add("        </y:ShapeNode>");
            list.Add("      </data>");
            list.Add("    </node>");
        }
        private void whiteNode(List<string> list, string ifc, int x, int y)
        {
            list.Add("    <node id=\"" + ifc + "\">");
            list.Add("      <data key=\"d5\"/>");
            list.Add("      <data key=\"d6\">");
            list.Add("        <y:ShapeNode>");
            list.Add("          <y:Geometry height=\"50.0\" width=\"100.0\" x=\"" + x.ToString() + ".0\" y=\"" + y.ToString() + ".0\"/>");
            list.Add("          <y:Fill hasColor=\"false\" transparent =\"false\"/>");
            list.Add("          <y:BorderStyle color=\"#000000\" raised=\"false\" type =\"line\" width=\"1.0\"/>");
            list.Add("          <y:NodeLabel alignment=\"center\" autoSizePolicy=\"content\" fontFamily=\"Dialog\" fontSize=\"10\" fontStyle=\"plain\" hasBackgroundColor=\"false\" hasLineColor=\"false\" height=\"18.701171875\" horizontalTextPosition=\"center\" iconTextGap=\"4\" modelName=\"custom\" textColor=\"#000000\" verticalTextPosition=\"bottom\" visible=\"true\" width=\"40.03515625\" x=\"29.982421875\" xml:space=\"preserve\" y=\"15.6494140625\">" + ifc + "<y:LabelModel><y:SmartNodeLabelModel distance=\"4.0\"/></y:LabelModel><y:ModelParameter><y:SmartNodeLabelModelParameter labelRatioX=\"0.0\" labelRatioY =\"0.0\" nodeRatioX =\"0.0\" nodeRatioY =\"0.0\" offsetX =\"0.0\" offsetY =\"0.0\" upX =\"0.0\" upY =\" - 1.0\" /></y:ModelParameter></y:NodeLabel>");
            list.Add("          <y:Shape type=\"rectangle\"/>");
            list.Add("        </y:ShapeNode>");
            list.Add("      </data>");
            list.Add("    </node>");
        }
        private void edge(List<string> list, string source, string target)
        {
            list.Add("    <edge id=\""+ source +"-"+ target + "\" source=\"" + source + "\" target=\"" + target + "\">");
            list.Add("      <data key=\"d9\"/>");
            list.Add("      <data key=\"d10\">");
            list.Add("        <y:PolyLineEdge>");
            list.Add("          <y:Path sx=\"0.0\" sy=\"0.0\" tx=\"0.0\" ty=\"0.0\"/>");
            list.Add("          <y:LineStyle color=\"#000000\" type=\"line\" width=\"1.0\"/>");
            list.Add("          <y:Arrows source=\"none\" target=\"standard\"/>");
            list.Add("          <y:BendStyle smoothed=\"false\"/>");
            list.Add("        </y:PolyLineEdge>");
            list.Add("      </data>");
            list.Add("    </edge>");
        }

        /*Fonctions pour le clic droit*/
        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string value = listBox10.SelectedItem.ToString();
            string name = null;

            if (value.Contains("(") == false) { name = value; } else { name = value.Substring(0, value.IndexOf("(") - 1); }

            
            /*Initialisation*/
            textBox2.Text = "";

            /*General informations*/
            IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                              where (string)ifcclass.Attribute("name") == name
                                              select ifcclass;

            if (selection != null)
            {
                string line1 = "# GENERAL INFORMATIONS\r\n";
                string line2 = "\r\n";
                string line3 = "Class name : " + selection.ElementAt(0).Attribute("name").Value + "\r\n";
                string line4 = "Class type : " + selection.ElementAt(0).Attribute("type").Value + "\r\n";
                textBox2.Text = textBox2.Text + line1 + line2 + line3 + line4;
            }

            /*Natural language names*/
            selection = from ifcclass in xml.Elements("list").Elements("class")
                                              where (string)ifcclass.Attribute("name") == name
                                              select ifcclass;

            if (selection != null)
            {
                string line1 = "__________________________________________________\r\n";
                string line2 = "\r\n";
                string line3 = "# NATURAL LANGUAGE NAME(S)\r\n";
                string line4 = "\r\n";
                textBox2.Text = textBox2.Text + line1 + line2 + line3 + line4;

                if (selection.Elements("en").Count() > 0) { textBox2.Text = textBox2.Text + "EN : " + selection.Elements("en").ElementAt(0).Value + "\r\n"; }
                if (selection.Elements("fr").Count() > 0) { textBox2.Text = textBox2.Text + "FR : " + selection.Elements("fr").ElementAt(0).Value + "\r\n"; }
                if (selection.Elements("de").Count() > 0) { textBox2.Text = textBox2.Text + "DE : " + selection.Elements("de").ElementAt(0).Value + "\r\n"; }
            }

            /*Superclass*/
            selection = from ifcclass in xml.Elements("list").Elements("class")
                        where (string)ifcclass.Attribute("name") == name
                        select ifcclass;

            if (selection != null)
            {
                List<XElement> list = selection.Elements("sup").ToList();
                if (list.Count() > 0)
                {
                    string line1 = "__________________________________________________\r\n";
                    string line2 = "\r\n";
                    string line3 = "# SUPERCLASS\r\n";
                    string line4 = "\r\n";
                    textBox2.Text = textBox2.Text + line1 + line2 + line3 + line4;
                    foreach (XElement element in list)
                    {
                        textBox2.Text = textBox2.Text + element.Attribute("name").Value + "\r\n";
                    }
                }
            }

            /*Subclass*/
            selection = from ifcclass in xml.Elements("list").Elements("class")
                        where (string)ifcclass.Attribute("name") == name
                        select ifcclass;

            if (selection != null)
            {
                List<XElement> list = selection.Elements("sub").ToList();
                if (list.Count() > 0)
                {
                    string line1 = "__________________________________________________\r\n";
                    string line2 = "\r\n";
                    string line3 = "# SUBCLASSE(S)\r\n";
                    string line4 = "\r\n";
                    textBox2.Text = textBox2.Text + line1 + line2 + line3 + line4;
                    foreach (XElement element in list)
                    {
                        textBox2.Text = textBox2.Text + element.Attribute("name").Value + "\r\n";
                    }
                }
            }

            /*Relations and inverse relations*/
            selection = from ifcclass in xml.Elements("list").Elements("class")
                        where (string)ifcclass.Attribute("name") == name
                        select ifcclass;

            if (selection != null)
            {
                List<XElement> list1 = selection.Elements("direct_att").ToList();
                List<XElement> list2 = selection.Elements("inverse_att").ToList();
                if (list1.Count() > 0 || list2.Count() > 0)
                {
                    string line1 = "__________________________________________________\r\n";
                    string line2 = "\r\n";
                    string line3 = "# DIRECT (+) AND INVERSE ATTRIBUTE(S) (-)\r\n";
                    string line4 = "\r\n";
                    textBox2.Text = textBox2.Text + line1 + line2 + line3 + line4;

                    if (list1.Count() > 0)
                    {
                        foreach (XElement element in list1)
                        {
                            textBox2.Text = textBox2.Text + "+ " + element.Attribute("name").Value + "\r\n";
                        }
                    }
                    if (list2.Count() > 0)
                    {
                        foreach (XElement element in list2)
                        {
                            textBox2.Text = textBox2.Text + "- " + element.Attribute("name").Value + "\r\n";
                        }
                    }
                }
            }

            /*Enumerated type constant*/
            selection = from ifcclass in xml.Elements("list").Elements("class")
                        where (string)ifcclass.Attribute("name") == name
                        select ifcclass;

            if (selection != null)
            {
                List<XElement> list = selection.Elements("constant").ToList();
                if (list.Count() > 0)
                {
                    string line1 = "__________________________________________________\r\n";
                    string line2 = "\r\n";
                    string line3 = "# ENUMERATION CONSTANTS\r\n";
                    string line4 = "\r\n";
                    textBox2.Text = textBox2.Text + line1 + line2 + line3 + line4;
                    foreach (XElement element in list)
                    {
                        textBox2.Text = textBox2.Text + element.Attribute("name").Value + "\r\n";
                    }
                }
            }

            /*Select type constant*/
            selection = from ifcclass in xml.Elements("list").Elements("class")
                        where (string)ifcclass.Attribute("name") == name
                        select ifcclass;

            if (selection != null)
            {
                List<XElement> list = selection.Elements("type").ToList();
                if (list.Count() > 0)
                {
                    string line1 = "__________________________________________________\r\n";
                    string line2 = "\r\n";
                    string line3 = "# SELECTABLE TYPES\r\n";
                    string line4 = "\r\n";
                    textBox2.Text = textBox2.Text + line1 + line2 + line3 + line4;
                    foreach (XElement element in list)
                    {
                        textBox2.Text = textBox2.Text + element.Attribute("name").Value + "\r\n";
                    }
                }
            }

            /*Related class*/
            selection = from ifcclass in xml.Elements("list").Elements("class")
                        where (string)ifcclass.Attribute("name") == name
                        select ifcclass;

            if (selection != null)
            {
                List<XElement> list = selection.Elements("relatedclass").ToList();
                if (list.Count() > 0)
                {
                    string line1 = "__________________________________________________\r\n";
                    string line2 = "\r\n";
                    string line3 = "# RELATED CLASS\r\n";
                    string line4 = "\r\n";
                    textBox2.Text = textBox2.Text + line1 + line2 + line3 + line4;
                    foreach (XElement element in list)
                    {
                        textBox2.Text = textBox2.Text + element.Attribute("name").Value + "\r\n";
                    }
                }
            }

            /*Property*/
            selection = from ifcclass in xml.Elements("list").Elements("class")
                        where (string)ifcclass.Attribute("name") == name
                        select ifcclass;

            if (selection != null)
            {
                List<XElement> list = selection.Elements("property").ToList();
                if (list.Count() > 0)
                {
                    string line1 = "__________________________________________________\r\n";
                    string line2 = "\r\n";
                    string line3 = "# PROPERTIES\r\n";
                    string line4 = "\r\n";
                    textBox2.Text = textBox2.Text + line1 + line2 + line3 + line4;
                    foreach (XElement element in list)
                    {
                        textBox2.Text = textBox2.Text + element.Attribute("name").Value + "\r\n";
                    }
                }
            }

            /*Quantity*/
            selection = from ifcclass in xml.Elements("list").Elements("class")
                        where (string)ifcclass.Attribute("name") == name
                        select ifcclass;

            if (selection != null)
            {
                List<XElement> list = selection.Elements("quantity").ToList();
                if (list.Count() > 0)
                {
                    string line1 = "__________________________________________________\r\n";
                    string line2 = "\r\n";
                    string line3 = "# QUANTITIES\r\n";
                    string line4 = "\r\n";
                    textBox2.Text = textBox2.Text + line1 + line2 + line3 + line4;
                    foreach (XElement element in list)
                    {
                        textBox2.Text = textBox2.Text + element.Attribute("name").Value + "\r\n";
                    }
                }
            }
        }
        private void pathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox10.SelectedItem != null)
            {
                string value = listBox10.SelectedItem.ToString();
                string name = null;

                if (value.Contains("(") == false)
                {
                    if (value.ToLower().Contains("ifc") == true) { name = listBox10.SelectedItem.ToString(); } else { MessageBox.Show("No path founded for " + value, "IfcExplorer"); }
                }
                else
                {
                    value = value.Substring(0, value.IndexOf("(") - 1);
                    if (value.ToLower().Contains("ifc") == true) { name = value; } else { MessageBox.Show("No path founded for " + value, "IfcExplorer"); }
                }

                path.Clear();
                path.Add(name);

                if (name != null)
                {

                    /*Create path list*/
                    for (int i = 0; i <= 9; ++i)
                    {
                        IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                                          where (string)ifcclass.Attribute("name") == path.ElementAt(0).ToString()
                                                          select ifcclass;
                        if (selection != null)
                        {
                            if (selection.Elements("sup").Count() > 0)
                            {
                                if (selection.Elements("sup").ElementAt(0).Attribute("name").Value != path.ElementAt(0).ToString())
                                {
                                    path.Insert(0, selection.Elements("sup").ElementAt(0).Attribute("name").Value);
                                }
                            }
                        }
                    }

                    /*Cases*/
                    if (path.ElementAt(0) == "IfcRoot")
                    {
                        radioButton1.Checked = true; radioButton1_Click(sender, e);
                        for (int i = path.Count(); i < 9; ++i) { listbox[i].Items.Clear(); listbox[i].Refresh(); }
                        foreach (string element in path)
                        {
                            listbox[path.IndexOf(element)].SelectedItem = element;
                        }
                        listbox[path.Count() - 1].Focus();
                    }
                    else
                    {
                        IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("group").Elements("res")
                                                          where (string)ifcclass.Attribute("name") == path.ElementAt(0).ToString()
                                                          select ifcclass;
                        if (selection.Count() > 0)
                        {
                            path.Insert(0, selection.Ancestors().ElementAt(0).Attribute("name").Value);
                            radioButton2.Checked = true; radioButton2_Click(sender, e);
                            for (int i = path.Count(); i < 9; ++i) { listbox[i].Items.Clear(); listbox[i].Refresh(); }
                            foreach (string element in path)
                            {
                                listbox[path.IndexOf(element)].SelectedItem = element;
                            }
                            listbox[path.Count() - 1].Focus();
                        }
                        else
                        { MessageBox.Show("No path founded for " + value, "IfcExplorer"); }
                    }
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            pathToolStripMenuItem.Enabled = false;
            detailsToolStripMenuItem.Enabled = false;

            if (listBox10.SelectedItem != null)
            {
                detailsToolStripMenuItem.Enabled = true;
                string value = listBox10.SelectedItem.ToString();
                string name = null;

                if (value.Contains("(") == false)
                {
                    name = listBox10.SelectedItem.ToString();
                }
                else
                {
                    name = value.Substring(0, value.IndexOf("(") - 1);
                }

                path.Clear();
                path.Add(name);

                IEnumerable<XElement> selection = from ifcclass in xml.Elements("list").Elements("class")
                                                  where (string)ifcclass.Attribute("name") == name
                                                  select ifcclass;

                if (selection != null)
                {
                    if (selection.ElementAt(0).Attribute("type").Value.ToString() == "Entity") { pathToolStripMenuItem.Enabled = true; }
                }
            }
        }
        private void listBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = new Point((Size)e.Location);
            if (listBox1.IndexFromPoint(pt.X, pt.Y) != -1) { toolTip1.SetToolTip(listBox1, listBox1.Items[listBox1.IndexFromPoint(pt.X, pt.Y)].ToString()); } else { toolTip1.SetToolTip(listBox1, ""); }
        }

        private void listBox2_MouseMove(object sender, MouseEventArgs e)
        {
            /*
            Point pt = new Point((Size)e.Location);
            if (listBox2.IndexFromPoint(pt.X, pt.Y) != -1)
            {
                
                if (listBox2.IndexFromPoint(pt.X, pt.Y) != activei[1])
                {
                    Console.WriteLine(activei[1] + " " + listBox2.IndexFromPoint(pt.X, pt.Y));
                    toolTip1.SetToolTip(listBox2, listBox2.Items[listBox2.IndexFromPoint(pt.X, pt.Y)].ToString());
                    activei[1] = listBox2.IndexFromPoint(pt.X, pt.Y);
                }
            } else { toolTip1.SetToolTip(listBox2, ""); } */

            Point pt = new Point((Size)e.Location);
            if (listBox2.IndexFromPoint(pt.X, pt.Y) != -1) { toolTip1.SetToolTip(listBox2, listBox2.Items[listBox2.IndexFromPoint(pt.X, pt.Y)].ToString()); } else { toolTip1.SetToolTip(listBox2, ""); }

        }
        private void listBox3_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = new Point((Size)e.Location);
            if (listBox3.IndexFromPoint(pt.X, pt.Y) != -1) { toolTip1.SetToolTip(listBox3, listBox3.Items[listBox3.IndexFromPoint(pt.X, pt.Y)].ToString()); } else { toolTip1.SetToolTip(listBox3, ""); }
        }

        private void listBox4_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = new Point((Size)e.Location);
            if (listBox4.IndexFromPoint(pt.X, pt.Y) != -1) { toolTip1.SetToolTip(listBox4, listBox4.Items[listBox4.IndexFromPoint(pt.X, pt.Y)].ToString()); } else { toolTip1.SetToolTip(listBox4, ""); }
        }
        private void listBox5_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = new Point((Size)e.Location);
            if (listBox5.IndexFromPoint(pt.X, pt.Y) != -1) { toolTip1.SetToolTip(listBox5, listBox5.Items[listBox5.IndexFromPoint(pt.X, pt.Y)].ToString()); } else { toolTip1.SetToolTip(listBox5, ""); }
        }
        private void listBox6_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = new Point((Size)e.Location);
            if (listBox6.IndexFromPoint(pt.X, pt.Y) != -1) { toolTip1.SetToolTip(listBox6, listBox6.Items[listBox6.IndexFromPoint(pt.X, pt.Y)].ToString()); } else { toolTip1.SetToolTip(listBox6, ""); }
        }
        private void listBox7_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = new Point((Size)e.Location);
            if (listBox7.IndexFromPoint(pt.X, pt.Y) != -1) { toolTip1.SetToolTip(listBox7, listBox7.Items[listBox7.IndexFromPoint(pt.X, pt.Y)].ToString()); } else { toolTip1.SetToolTip(listBox7, ""); }
        }
        private void listBox8_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = new Point((Size)e.Location);
            if (listBox8.IndexFromPoint(pt.X, pt.Y) != -1) { toolTip1.SetToolTip(listBox8, listBox8.Items[listBox8.IndexFromPoint(pt.X, pt.Y)].ToString()); } else { toolTip1.SetToolTip(listBox8, ""); }
        }
        private void listBox9_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = new Point((Size)e.Location);
            if (listBox9.IndexFromPoint(pt.X, pt.Y) != -1) { toolTip1.SetToolTip(listBox9, listBox9.Items[listBox9.IndexFromPoint(pt.X, pt.Y)].ToString()); } else { toolTip1.SetToolTip(listBox9, ""); }
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= 8; ++i) { listbox[i].Items.Clear(); listbox[i].Refresh(); }
            listBox1.Items.Add("IfcRoot");
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= 8; ++i) { listbox[i].Items.Clear(); listbox[i].Refresh(); }
            listBox1.Items.Add("IfcActorResource");
            listBox1.Items.Add("IfcApprovalResource");
            listBox1.Items.Add("IfcConstraintResource");
            listBox1.Items.Add("IfcCostResource");
            listBox1.Items.Add("IfcDateTimeResource");
            listBox1.Items.Add("IfcExternalReferenceResource");
            listBox1.Items.Add("IfcGeometricConstraintResource");
            listBox1.Items.Add("IfcGeometricModelResource");
            listBox1.Items.Add("IfcGeometryResource");
            listBox1.Items.Add("IfcMaterialResource");
            listBox1.Items.Add("IfcMeasureResource");
            listBox1.Items.Add("IfcPresentationAppearanceResource");
            listBox1.Items.Add("IfcPresentationDefinitionResource");
            listBox1.Items.Add("IfcPresentationOrganizationResource");
            listBox1.Items.Add("IfcProfileResource");
            listBox1.Items.Add("IfcPropertyResource");
            listBox1.Items.Add("IfcQuantityResource");
            listBox1.Items.Add("IfcRepresentationResource");
            listBox1.Items.Add("IfcStructuralLoadResource");
            listBox1.Items.Add("IfcTopologyResource");
            listBox1.Items.Add("IfcUtilityResource");
        }
    }
}
