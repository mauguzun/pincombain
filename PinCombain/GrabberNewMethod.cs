using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinCombain
{
    class GrabberNewMethod  : MakeGrabber
    {

        public string baseUrl = "https://www.pinterest.com/search/pins/?q=popular";
        public List<Tag> tags;
        public GrabberNewMethod() 
        {
            this.tags= new List<Tag>();
        }

        public void Start()
        {

            if (_domains.Count == 0)
            {
                Console.WriteLine("please load domain");
                return;
            }


            while (true)
            {
                GetTag();
                DoGrab();
            }


        }
        protected void  GetTag()
        {
            if (this.tags.Count == 0)
            {
                this.Driver.Url = this.baseUrl;
            }
            else
            {
                Tag tag = this.tags.Where(x => x.Visited == false).FirstOrDefault();
                tag.Visited = false;
                this.Driver.Url = tag.Url;

            }



            var grabbedTags = Driver.FindElementsByCssSelector(".SearchImprovementsBar-OuterScrollContainer div a");
            if(grabbedTags.Count > 0)
            {
                foreach (var item in grabbedTags)
                {
                    string text = item.Text;
                    string url = this.Driver.Url + "%20" + text;
                    if (!this.tags.Where(x=>x.Url == url).Any())
                    {
                        this.tags.Add(new Tag() { Url = url });
                    }
                }
            }
            Console.WriteLine(this.tags.Count() / this.tags.Where(x => x.Visited == false).Count());


        }


    }

    class Tag
    {
        public string Url { get; set; }
        public bool Visited { get; set; } = false;
    }
}
