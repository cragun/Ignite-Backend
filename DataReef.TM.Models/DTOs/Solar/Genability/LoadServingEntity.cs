using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// Load Serving Entity (LSE) is the industry term for what most people would call a utility, or an electric company.
    /// </summary>
    public class LoadServingEntity:BaseResponse
    {
        /// <summary>
        /// Unique Genability ID (primary key) for each LSE.
        /// </summary>
        public long lseId                        { get; set; }
                   
        /// <summary>
        /// Published name of the company.
        /// </summary>                      
        public string name                       { get; set; }
                  
        /// <summary>
        /// Short code (an alternate key). For US companies this is the EIA ID.
        /// </summary>                       
        public string code                       { get; set; }
                             
        /// <summary>
        /// The URL to the home page of the LSE website.
        /// </summary>            
        public string websiteHome                { get; set; }
                   
        /// <summary>
        /// Whether the offerings are bundled or one of energy or delivery.
        /// </summary>                      
        public string offeringType               { get; set; }
                      
        /// <summary>
        /// Ownership structure. Most common values are "INVESTOR", "COOP", "MUNI". 
        /// Other include "FEDERAL", "POLITICAL_SUBDIVISION", "RETAIL_ENERGY_MARKETER", "WHOLESALE_ENERGY_MARKETER", "TRANSMISSION", "STATE", "UNREGULATED".
        /// </summary>                   
        public string ownership                  { get; set; }

        /// <summary>
        /// Service types offered to any customer. Current values include "ELECTRICITY" and "SOLAR_PV".
        /// </summary>                                         
        public string serviceTypes               { get; set; }
                        
        /// <summary>
        /// Annual total revenue in local currency (e.g. USD).
        /// </summary>                 
        public string totalRevenues              { get; set; }
                    
        /// <summary>
        /// Annual total sales for the appropriate unit of quantity (e.g. MWh).
        /// </summary>                     
        public string totalSales                 { get; set; }
            
        /// <summary>
        /// Total customer count.
        /// </summary>                             
        public string totalCustomers             { get; set; }
                  
        /// <summary>
        /// Service types offered to residential customers. Current values include "ELECTRICITY" and "SOLAR_PV". Blank means not offered.
        /// </summary>                       
        public string residentialServiceTypes    { get; set; }
                     
        /// <summary>
        /// Annual residential revenue in local currency (e.g. USD).
        /// </summary>                    
        public string residentialRevenues        { get; set; }
                  
        /// <summary>
        /// Annual residential sales for the appropriate unit of quantity (e.g. MWh).
        /// </summary>                       
        public string residentialSales           { get; set; }
                
        /// <summary>
        /// Residential customer count.
        /// </summary>                         
        public string residentialCustomers       { get; set; }
                     
        /// <summary>
        /// Service types offered to commercial customers. Current values include "ELECTRICITY" and "SOLAR_PV". Blank means not offered.
        /// </summary>                    
        public string commercialServiceTypes     { get; set; }

        /// <summary>
        /// Annual commercial revenue in local currency (e.g. USD).
        /// </summary>                                
        public string commercialRevenues         { get; set; }
                 
        /// <summary>
        /// Annual commercial sales for the appropriate unit of quantity (e.g. MWh).
        /// </summary>            
        public string commercialSales            { get; set; }
                
        /// <summary>
        /// Commercial customer count.
        /// </summary>                  
        public string commercialCustomers        { get; set; }
               
        /// <summary>
        /// Service types offered to industrial customers. Current values include "ELECTRICITY" and "SOLAR_PV". Blank means not offered.
        /// </summary>              
        public string industrialServiceTypes     { get; set; }
               
        /// <summary>
        /// Annual industrial revenue in local currency (e.g. USD).
        /// </summary>                  
        public string industrialRevenues         { get; set; }
                
        /// <summary>
        /// Annual industrial sales for the appropriate unit of quantity (e.g. MWh).
        /// </summary>                    
        public string industrialSales            { get; set; }
                
        /// <summary>
        /// Industrial customer count.
        /// </summary>                        
        public string industrialCustomers        { get; set; }

        /// <summary>
        /// Service types offered to transportation customers (such as municipal bus services, regional mass transit etc). 
        /// Current values include "ELECTRICITY" and "SOLAR_PV". Blank means not offered.
        /// </summary>
        public string transportationServiceTypes { get; set; }

        /// <summary>
        /// Annual transportation revenue in local currency (e.g. USD).
        /// </summary>
        public string transportationRevenues     { get; set; }

        /// <summary>
        /// Annual transportation sales for the appropriate unit of quantity (e.g. MWh).
        /// </summary>
        public string transportationSales        { get; set; }

        /// <summary>
        /// Transportation customer count.
        /// </summary>
        public string transportationCustomers    { get; set; }
    }
}
