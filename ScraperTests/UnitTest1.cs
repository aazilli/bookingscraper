using NUnit.Framework;
using HtmlAgilityPack;


namespace ScraperTests
{
    public class Tests
    {
        
        [Test]
        public void Name_Test()
        {
            var path = @"booking.html";
            var doc = new HtmlDocument();
            doc.Load(path);
            var name = Scraper.Parser.get_name(doc);

            Assert.AreEqual(name, "Kempinski Hotel Bristol Berlin");
        }

        [Test]
        public void Address_Test()
        {
            var path = @"booking.html";
            var doc = new HtmlDocument();
            doc.Load(path);
            var address = Scraper.Parser.get_address(doc);

            Assert.AreEqual(address["number"], "27");
            Assert.AreEqual(address["street"], "Kurfürstendamm");
            Assert.AreEqual(address["postal_code"], "10719");
            Assert.AreEqual(address["city"], "Berlin");
            Assert.AreEqual(address["city_region"], "Charlottenburg-Wilmersdorf");
            Assert.AreEqual(address["country"], "Germany");
        }

        [Test]
        public void Stars_Test()
        {
            var path = @"booking.html";
            var doc = new HtmlDocument();
            doc.Load(path);

            Assert.AreEqual(Scraper.Parser.get_stars(doc), 5);
        }

        [Test]
        public void Score_Test()
        {
            var path = @"booking.html";
            var doc = new HtmlDocument();
            doc.Load(path);
            var score = Scraper.Parser.get_score(doc);

            Assert.AreEqual(score["scoreword"], "Very good");
            Assert.AreEqual(score["score_num"], 8.3);
            Assert.AreEqual(score["review_count"], 1401);
        }

        [Test]
        public void Description_Test()
        {
            var path = @"booking.html";
            var doc = new HtmlDocument();
            doc.Load(path);
            var description = Scraper.Parser.get_description(doc);

            StringAssert.StartsWith("This 5-star hotel on Berlin’s Kurfürstendamm shopping street", description);
            StringAssert.EndsWith("Hotel Rooms: 301, Hotel Chain:Kempinski", description);
            Assert.AreEqual(description.Length, 1080);
        }

        [Test]
        public void Rooms_Test()
        {
            var path = @"booking.html";
            var doc = new HtmlDocument();
            doc.Load(path);
            var rooms = Scraper.Parser.get_rooms(doc);

            Assert.Contains("Suite with Balcony", rooms);
            Assert.Contains("Classic Double or Twin Room", rooms);
            Assert.Contains("Superior Double or Twin Room", rooms);
            Assert.Contains("Deluxe Double Room", rooms);
            Assert.Contains("Deluxe Business Suite", rooms);
            Assert.Contains("Junior Suite", rooms);
            Assert.Contains("Family Room", rooms);
        }

        [Test]
        public void Alt_Hotels_Test()
        {
            var path = @"booking.html";
            var doc = new HtmlDocument();
            doc.Load(path);
            var alt_hotels = Scraper.Parser.get_alt_hotels(doc);

            Assert.AreEqual(alt_hotels[0]["alt_review_count"], 1933);
            Assert.AreEqual(alt_hotels[0]["alt_name"], "Hotel Adlon Kempinski Berlin");
            Assert.AreEqual(alt_hotels[0]["alt_description"], 
                "The quintessence of luxury lodging, the Adlon is a legendary 5-star hotel situated in Berlin’s Mitte, beside the Brandenburg Gate.");
            Assert.AreEqual(alt_hotels[0]["rating"], 9.4);
            Assert.AreEqual(alt_hotels[0]["alt_scoreword"], "Superb");
            Assert.AreEqual(alt_hotels[0]["alt_viewer_count"], 21);

            Assert.AreEqual(alt_hotels[1]["alt_review_count"], 1460);
            Assert.AreEqual(alt_hotels[1]["alt_name"], "Grand Hyatt Berlin");
            Assert.AreEqual(alt_hotels[1]["alt_description"],
                "This 5-star hotel has a large rooftop spa and pool with spectacular views of Berlin.");
            Assert.AreEqual(alt_hotels[1]["rating"], 9.1);
            Assert.AreEqual(alt_hotels[1]["alt_scoreword"], "Superb");
            Assert.AreEqual(alt_hotels[1]["alt_viewer_count"], 8);

            Assert.AreEqual(alt_hotels[2]["alt_review_count"], 1497);
            Assert.AreEqual(alt_hotels[2]["alt_name"], "Sofitel Berlin Kurfürstendamm");
            Assert.AreEqual(alt_hotels[2]["alt_description"],
                "Just 100 metres from the Kurfürstendamm boulevard, this 5-star design hotel offers air-conditioned rooms, free Wi-Fi and a French restaurant. Guests have free use of the spa and gym.");
            Assert.AreEqual(alt_hotels[2]["rating"], 9.0);
            Assert.AreEqual(alt_hotels[2]["alt_scoreword"], "Superb");
            Assert.AreEqual(alt_hotels[2]["alt_viewer_count"], 4);

            Assert.AreEqual(alt_hotels[3]["alt_review_count"], 2700);
            Assert.AreEqual(alt_hotels[3]["alt_name"], "Hilton Berlin");
            Assert.AreEqual(alt_hotels[3]["alt_description"],
                "This centrally located hotel on Berlin’s historic Gendarmenmarkt Square features luxurious rooms, an exclusive spa and 2 restaurants with stunning views of the German Cathedral.");
            Assert.AreEqual(alt_hotels[3]["rating"], 8.5);
            Assert.AreEqual(alt_hotels[3]["alt_scoreword"], "Very good");
            Assert.AreEqual(alt_hotels[3]["alt_viewer_count"], 6);
        }

    }
}