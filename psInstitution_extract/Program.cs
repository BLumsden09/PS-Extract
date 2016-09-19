using System;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Linq;



namespace psInstituion_extract
{
    class Program
    {
        static void Main(string[] args)
        {
            using (
                var conn = new SqlConnection("Server=0;Database=0;User ID=0;Password= 0;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;")
                )
            {
                conn.Open();

                bool quit = false;
                string choice;
                SqlCommand cmd = new SqlCommand();
                Console.WriteLine("<------------------------");
                Console.WriteLine("    PS Institution");
                Console.WriteLine("------------------------>");

                /*Query Post Secondary database table based on user input*/
                while (!quit)
                {
                    Console.WriteLine("Select by public, private, or none?");
                    string category = Console.ReadLine();
                    if (category == "public")
                    {
                        Console.WriteLine("Select by code, date, both, or none?");
                        choice = Console.ReadLine();
                        switch (choice)
                        {
                            case "code":
                                Console.WriteLine("Select by code1 or code2?");
                                string codeCol = Console.ReadLine();
                                Console.WriteLine("Enter desired code");
                                string code = Console.ReadLine();
                                cmd = new SqlCommand("SELECT * FROM psInstitution WHERE (psInstitution.psi_" + @codeCol + "='" + @code + "') AND psi_sector ='" + @category + "' FOR XML PATH('psInstitution'), ROOT('psInstitutiontype')", conn);
                                quit = true;
                                break;

                            case "date":
                                Console.WriteLine("Enter desired date using the format MM/DD/YYYY.");
                                string date = Console.ReadLine();
                                cmd = new SqlCommand("SELECT * FROM psInstitution WHERE (psInstitution.psi_date_created >= '" + @date + "' OR psInstitution.psi_date_updated >= '" + @date + "') AND psi_sector ='" + @category + "' FOR XML PATH('psInstitution'), ROOT('psInstitutiontype')", conn);
                                quit = true;
                                break;

                            case "both":
                                cmd = new SqlCommand("SELECT * FROM psInstitution WHERE psi_sector ='" + @category + "'FOR XML PATH('psInstitution'), ROOT('psInstitutiontype')", conn);
                                quit = true;

                                break;

                            case "none":
                                cmd = new SqlCommand("SELECT * FROM psInstitution WHERE psi_sector ='" + @category + "' FOR XML PATH('psInstitution'), ROOT('psInstitutiontype')", conn);
                                quit = true;
                                break;

                            default:
                                Console.WriteLine("Unknown Command " + choice);
                                continue;
                        }
                    }
                    else if (category == "private")
                    {
                        Console.WriteLine("Select by code, date, both, or none?");
                        choice = Console.ReadLine();
                        switch (choice)
                        {
                            case "code":
                                Console.WriteLine("Select by code1 or code2?");
                                string codeCol = Console.ReadLine();
                                Console.WriteLine("Enter desired code");
                                string code = Console.ReadLine();
                                cmd = new SqlCommand("SELECT * FROM psInstitution WHERE (psInstitution.psi_" + @codeCol + "='" + @code + "') AND psi_sector ='" + @category + "' FOR XML PATH('psInstitution'), ROOT('psInstitutiontype')", conn);
                                quit = true;
                                break;

                            case "date":
                                Console.WriteLine("Enter desired date using the format MM/DD/YYYY.");
                                string date = Console.ReadLine();
                                cmd = new SqlCommand("SELECT * FROM psInstitution WHERE (psInstitution.psi_date_created >= '" + @date + "' OR psInstitution.psi_date_updated >= '" + @date + "') AND psi_sector ='" + @category + "' FOR XML PATH('psInstitution'), ROOT('psInstitutiontype')", conn);
                                quit = true;
                                break;

                            case "both":
                                cmd = new SqlCommand("SELECT * FROM psInstitution WHERE psi_sector ='" + @category + "' FOR XML PATH('psInstitution'), ROOT('psInstitutiontype')", conn);
                                quit = true;

                                break;

                            case "none":
                                cmd = new SqlCommand("SELECT * FROM psInstitution WHERE psi_sector ='" + @category + "' FOR XML PATH('psInstitution'), ROOT('psInstitutiontype')", conn);
                                quit = true;
                                break;

                            default:
                                Console.WriteLine("Unknown Command " + choice);
                                continue;
                        }
                    }
                    else if (category == "none")
                    {
                        Console.WriteLine("Select by code, date, both, or none?");
                        choice = Console.ReadLine();
                        switch (choice)
                        {
                            case "code":
                                Console.WriteLine("Select by code1 or code2?");
                                string codeCol = Console.ReadLine();
                                Console.WriteLine("Enter desired code");
                                string code = Console.ReadLine();
                                cmd = new SqlCommand("SELECT * FROM psInstitution  WHERE (psInstitution.psi_" + @codeCol + "='" + @code + "') FOR XML PATH('psInstitution'), ROOT('psInstitutiontype')", conn);
                                quit = true;
                                break;

                            case "date":
                                Console.WriteLine("Enter desired date using the format MM/DD/YYYY.");
                                string date = Console.ReadLine();
                                cmd = new SqlCommand("SELECT * FROM psInstitution WHERE (psInstitution.psi_date_created >= '" + @date + "' OR psInstitution.psi_date_updated >= '" + @date + "') FOR XML PATH('psInstitution'), ROOT('psInstitutiontype')", conn);
                                quit = true;
                                break;

                            case "both":
                                cmd = new SqlCommand("SELECT * FROM psInstitution FOR XML PATH('psInstitution'), ROOT('psInstitutiontype')", conn);
                                quit = true;

                                break;

                            case "none":
                                cmd = new SqlCommand("SELECT * FROM psInstitution FOR XML PATH('psInstitution'), ROOT('psInstitutiontype')", conn);
                                quit = true;
                                break;

                            default:
                                Console.WriteLine("Unknown Command " + choice);
                                continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unknown Command " + category);
                        continue;
                    }
                }
                using (cmd)
                {
                    using (var reader = cmd.ExecuteXmlReader())
                    {
                        var doc = new XDocument();
                        try {
                            doc = XDocument.Load(reader);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine("There are no entries that match the given parameters.");
                        }
                        /*Create custom file name to show database table name and date created.*/
                        string path = @"psInstitution." + DateTime.Now.ToString("yyyyMMdd") + ".xml";
                        using (var writer = new StreamWriter(path))
                        {
                            XNamespace ns = "http://specification.sifassociation.org/Implementation/na/3.2/html/CEDS/PostSecondary/PostSecondary_psinstitution.html";
                            var root = new XElement(ns + "psInstitutiontype");
                            int count = 0;

                            foreach (var d in doc.Descendants("psInstitution"))
                            {
                                string delete;
                                string street;
                                string street2;
                                string id;

                                /*Delete flag modification*/
                                if ((string)d.Element("psi_delete_flag") == "Y")
                                {
                                    delete = "1";
                                }
                                else
                                {
                                    delete = "0";
                                }

                                /*street check for null*/
                                if ((string)d.Element("psi_streetLine2") == null)
                                {
                                    street2 = string.Empty;
                                    street = ((string)d.Element("psi_streetLine1") + street2);
                                }
                                else
                                {
                                    street2 = (string)d.Element("psi_streetLine2");
                                    street = ((string)d.Element("psi_streetLine1") + "," + street2);
                                }

                                /*campus check for null*/
                                if ((string)d.Element("psi_campus") == null)
                                {
                                    id = (string)d.Element("psi_ipedsID");
                                }
                                else
                                {
                                    id = ((string)d.Element("psi_ipedsID")+ "-" +(string)d.Element("psi_campus"));
                                }
                                count++;
                                /*Create custom XML file with query result*/
                                root.Add(new XElement(ns + "psInstitution",
                                            new XElement(ns + "refID", (string)d.Element("psi_refid")
                                            ),
                                            new XElement(ns + "directory",
                                                new XElement(ns + "institutionName", (string)d.Element("psi_name")),
                                                new XElement(ns + "levelOfInstitution", (string)d.Element("psi_level"))
                                                    ),
                                            new XElement(ns + "addressList",
                                                new XElement(ns + "address",
                                                    new XElement(ns + "street",
                                                        new XElement(ns + "line1", street)

                                                        ),
                                                    new XElement(ns + "city", (string)d.Element("psi_city")),
                                                    new XElement(ns + "stateProvince", (string)d.Element("psi_stateProvince")),
                                                    new XElement(ns + "postalCode", (string)d.Element("psi_postalCode")),
                                                    new XElement(ns + "county", (string)d.Element("psi_county"))
                                                    )
                                                ),
                                            new XElement(ns + "ipeds",
                                                new XElement(ns + "ipedsIdentifier", id)
                                                    ),
                                              new XElement(ns + "access",
                                                  new XElement(ns + "institutionIdentifier", (string)d.Element("psi_category"))
                                                  ),
                                            new XElement(ns + "delete", delete)
                                                )
                                            );
                            }

                            root.Save(writer);
                            Console.WriteLine("" + count + " Post Secondary Institution records written");
                            Console.ReadLine();
                        }


                    }
                }
            }

        }
    }
}

