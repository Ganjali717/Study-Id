using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyId.Data.DatabaseContext;

namespace StudyId.WebApplication.Tests.Base
{
    public static class Utilities
    {
        public static void InitializeDbForTests(StudyIdDbContext db)
        {
            // db.Messages.AddRange(GetSeedingMessages());
            // db.SaveChanges();
        }

        public static void ReinitializeDbForTests(StudyIdDbContext db)
        {
            //db.Messages.RemoveRange(db.Messages);
      
        }
    }
   
}
