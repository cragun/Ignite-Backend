using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class Geo
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class JobNimbusLeadRequestData
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string display_name { get; set; } 
        public string record_type_name { get; set; }
        public string status_name { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string city { get; set; }
        public string state_text { get; set; }
        public string zip { get; set; }
        public string customer { get; set; }
        public Geo geo { get; set; }
    }

    public class Location
    {
        public int id { get; set; }
    }

    public class JobNimbusLeadResponseData
    {
        public string type { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public Geo geo { get; set; }
        public string display_name { get; set; }
        public string customer { get; set; }
        public string country_name { get; set; }
        public int record_type { get; set; }
        public bool is_sub_contractor { get; set; }
        public int status { get; set; }
        public bool is_archived { get; set; }
        public bool is_lead { get; set; }
        public bool is_closed { get; set; }
        public List<object> tags { get; set; }
        public int recid { get; set; }
        public Location location { get; set; }
        public string company { get; set; }
        public string jnid { get; set; }
        public string created_by { get; set; }
        public string created_by_name { get; set; }
        public int date_created { get; set; }
        public int date_updated { get; set; }
        public int date_status_change { get; set; }
        public bool is_active { get; set; }
        public string record_type_name { get; set; }
        public string status_name { get; set; }
        public string number { get; set; }
        public int date_start { get; set; }
        public int date_end { get; set; }
        public int estimated_time { get; set; }
        public int actual_time { get; set; }
        public int task_count { get; set; }
        public int approved_estimate_total { get; set; }
        public int approved_invoice_total { get; set; }
        public int approved_invoice_due { get; set; }
        public int last_estimate { get; set; }
        public int last_invoice { get; set; }
        public int last_budget_gross_margin { get; set; }
        public int last_budget_gross_profit { get; set; }
        public int last_budget_revenue { get; set; }
        public bool is_user { get; set; }
    }

    public class related
    {
        public string id { get; set; }
    }

    public class AppointmentJobNimbusLeadRequestData
    { 
            public long date_start { get; set; } 
            public string title { get; set; }
            public List<related> related { get; set; }
            public int record_type { get; set; }
            public string record_type_name { get; set; }

    }

    public class AppointmentJobNimbusLeadResponseData
    {
        public string jnid { get; set; }
        public string customer { get; set; }
        public string type { get; set; }
        public List<related> related { get; set; }
        public Location location { get; set; }
        public string created_by { get; set; }
        public string created_by_name { get; set; }
        public int date_created { get; set; }
        public int date_updated { get; set; }
        public bool is_active { get; set; }
        public bool is_archived { get; set; }
        public int date_start { get; set; }
        public int date_end { get; set; }
        public List<object> tags { get; set; }
        public int recid { get; set; }
        public int estimated_time { get; set; }
        public int actual_time { get; set; }
        public string title { get; set; }
        public int record_type { get; set; }
        public string record_type_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string display_name { get; set; }
        public string country_name { get; set; }
        public bool is_sub_contractor { get; set; }
        public int status { get; set; }
        public bool is_lead { get; set; }
        public bool is_closed { get; set; }
        public string company { get; set; }
        public int date_status_change { get; set; }
        public string status_name { get; set; }
        public string number { get; set; }
        public int task_count { get; set; }
        public bool is_user { get; set; }
    }
}

