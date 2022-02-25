// See https://aka.ms/new-console-template for more information
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections;

namespace Scraper
{
    public class HotelPage
        // Defines HotelPage object that will be serialized to JSON
    {
        public string Name { get; set; }
        public Hashtable Address { get; set; }
        public int Stars { get; set; }
        public decimal Review_score { get; set; }
        public int Review_count { get; set; }
        public string Scoreword { get; set; }
        public string Description { get; set; }
        public List<string> Room_types { get; set; }
        public List<Hashtable> Alt_hotels { get; set; }
    }

    public class Parser
        // Contains methods for retrieving different parts of the HTML page
    {
        public static string get_name(HtmlDocument doc)
            // Retrieves name of hotel
        {
            var hotel = doc.GetElementbyId("hp_hotel_name");
            string hotel_name = hotel.InnerText.Trim();
            return hotel_name;
        }

        public static Hashtable get_address(HtmlDocument doc)
            // Retrieves address of hotel
        {
            var address = doc.GetElementbyId("hp_address_subtitle");
            string[] address_parts = address.InnerText.Split(",");
            string number = address_parts[0].Trim().Split(" ", 2)[1];
            string street = address_parts[0].Trim().Split(" ", 2)[0];
            string city_region = address_parts[1].Trim();
            string postal_code = address_parts[2].Trim().Split(" ", 2)[0];
            string city = address_parts[2].Trim().Split(" ", 2)[1];
            string country = address_parts[3].Trim();

            var address_hash = new Hashtable()
            {
                {"number", number},
                {"street", street},
                {"city_region", city_region},
                {"postal_code", postal_code},
                {"city", city},
                {"country", country},
            };

            return address_hash;
        }

        public static int get_stars(HtmlDocument doc)
            // Retrieves number of stars of hotel
        {

            var stars_section = doc.DocumentNode.SelectSingleNode("//i[contains(@class, 'stars')]");

            int stars = 0;

            if (stars_section.HasClass("ratings_stars_5"))
            {
                stars = 5;
            }

            if (stars_section.HasClass("ratings_stars_4"))
            {
                stars = 4;
            }

            if (stars_section.HasClass("ratings_stars_3"))
            {
                stars = 3;
            }

            if (stars_section.HasClass("ratings_stars_2"))
            {
                stars = 2;
            }

            if (stars_section.HasClass("ratings_stars_1"))
            {
                stars = 1;
            }


            return stars;
        }

        public static Hashtable get_score(HtmlDocument doc)
            // Retrieves score, scoreword, and number of reviews of hotel
        {
            var score = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'featured_review_score')]");
            string[] score_parts_raw = score.InnerText.Split(new char[]{'\n', '\r'});
            var score_parts_clean = new List<string>();

            foreach (string part in score_parts_raw)
            {
                if (part.Length > 0)
                {
                    score_parts_clean.Add(part);
                }
            }

            string scoreword = score_parts_clean[0];
            decimal score_num = Decimal.Parse(score_parts_clean[1].Split("/")[0]);
            int review_count = Int32.Parse(score.SelectSingleNode("//strong[contains(@class, 'count')]").InnerText.Trim());

            var score_hash = new Hashtable()
            {
                {"scoreword", scoreword},
                {"score_num", score_num},
                {"review_count", review_count}
            };

            return score_hash;
        }

        public static string get_description(HtmlDocument doc)
            // Retrieves description of hotel
        {
            var description = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'hotel_description_wrapper_exp')]");
            description.SelectSingleNode("//div[contains(@class, 'chain-content')]").Remove();
            var description_text = description.InnerText.Trim().Replace("\n", "").Replace("\r","");

            return description_text;
        }

        public static List<string> get_rooms(HtmlDocument doc)
            // Retrieves different room types available in hotel
        {
            var rooms = doc.DocumentNode.SelectNodes("//td[contains(@class, 'ftd')]");
            var rooms_list = new List<string>();

            foreach (HtmlAgilityPack.HtmlNode room in rooms)
            {
                rooms_list.Add(room.InnerText.Trim());
            }

            return rooms_list;
        }

        public static List<Hashtable> get_alt_hotels(HtmlDocument doc)
            // Retrieves alternate hotels and their respective values,
            // including name, description, rating, scoreword, review count, and # of viewers
        {
            var alt_hotel_list = new List<Hashtable>();
            var alt_hotels = doc.DocumentNode.SelectNodes("//td[contains(@class, 'althotelsCell')]");

            foreach (HtmlAgilityPack.HtmlNode alt_hotel in alt_hotels)
            {
                string alt_name = alt_hotel.SelectSingleNode(".//a[contains(@class, 'althotel_link')]").InnerText.Trim();
                string alt_description = alt_hotel.SelectSingleNode(".//span[contains(@class, 'hp_compset_description')]").InnerText.Trim();
                var info_row = alt_hotel.SelectSingleNode(".//div[contains(@class, 'alt_hotels_info_row')]");
                string alt_scoreword = info_row.SelectSingleNode(".//span[contains(@class, 'js--hp-scorecard-scoreword')]").InnerText.Trim();
                decimal alt_rating = Decimal.Parse(info_row.SelectSingleNode(".//span[contains(@class, 'js--hp-scorecard-scoreval')]").InnerText.Trim());
                int alt_review_count = Int32.Parse(info_row.SelectSingleNode(".//strong[contains(@class, 'count')]").InnerText.Trim());
                string alt_viewer_string = alt_hotel.SelectSingleNode(".//p[contains(@class, 'altHotels_most_recent_booking')]").InnerText.Trim();
                int alt_viewer_count = Int32.Parse(alt_viewer_string.Where(Char.IsDigit).ToArray());

                var alt_hotel_hash = new Hashtable()
                    {
                        {"alt_name", alt_name},
                        {"alt_description", alt_description},
                        {"alt_scoreword", alt_scoreword},
                        {"rating", alt_rating},
                        {"alt_review_count", alt_review_count},
                        {"alt_viewer_count", alt_viewer_count},
                    };

                alt_hotel_list.Add(alt_hotel_hash);
            }

            return alt_hotel_list;
        }
}

    class WriteJSON
        // Reads HTML file and constructs a HotelPage instance using the Parser methods on the HTML page
        // Then serializes HotelPage object and writes JSON to new file
    {
        static void Main()
        {
            var path = @"booking.html";
            var doc = new HtmlDocument();
            
            doc.Load(path);

            HotelPage hotel1 = new HotelPage()
            {
                Name = Parser.get_name(doc),
                Address = Parser.get_address(doc),
                Stars = Parser.get_stars(doc),
                Scoreword = (string)Parser.get_score(doc)["scoreword"],
                Review_score = (decimal)Parser.get_score(doc)["score_num"],
                Review_count = (int)Parser.get_score(doc)["review_count"],
                Description = Parser.get_description(doc),
                Room_types = Parser.get_rooms(doc),
                Alt_hotels = Parser.get_alt_hotels(doc)
            };

            string json_string = JsonConvert.SerializeObject(hotel1, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json_string);

            // Write JSON to file
            System.IO.File.WriteAllText("booking.json", json_string);
        }
    }
}