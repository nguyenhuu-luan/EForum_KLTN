using System.Text;
namespace EForumKLTN.Helpers
{
    public class MyUtil
    {
        public static string UploadHinh(IFormFile Hinh, string folder, IWebHostEnvironment env)
        {            
                var uploadPath = Path.Combine(env.WebRootPath, "Hinh", folder);              
                //TODO: Bug ngầm crash web khi tên img >50 ky tu -> tang so luong trong dbs hoặc xai` guid của asp.net (nay` vip hon)      

                // tạo folder nếu chưa có
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, Hinh.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create)) 
                {
                    Hinh.CopyTo(stream);
                }

                return Hinh.FileName;
            }                   

        public static string GenerateRamdomKey(int length = 5)
        {
            var pattern = @"qazwsxedcrfvtgbyhnujmiklopQAZWSXEDCRFVTGBYHNUJMIKLOP!";
            var sb = new StringBuilder();
            var rd = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }

            return sb.ToString();
        }
    }
}
