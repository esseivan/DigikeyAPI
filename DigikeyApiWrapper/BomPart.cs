using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigikeyApiWrapper
{
    /// <summary>
    /// Create a new Bill Of Materials (BOM)
    /// </summary>
    public class CreateBOM
    {
        // https://developer.digikey.com/products/mylists/mylists
        // Switch to MyList. BOM seems deprecated
    }

    public class BomPart
    {
        public string DigikeyPartNumber;
        public int Quantity;

        public BomPart() { }
    }

    public class BomResponse
    {
        public string BomId;
        public string BomTitle;
        public List<BomPart> Parts;
    }

    public class CreateBomRequest
    {
        public string BomTitle;
        public List<BomPart> Parts;
    }
}
