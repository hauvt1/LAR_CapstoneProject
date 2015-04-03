using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
namespace testConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SerialPort myserial = new SerialPort();
            myserial.PortName="COM11";
            myserial.BaudRate = 9600;
            myserial.Open();
            while (true)
            {
                char[] data = new char[13];
                      String tagsID = "";
                      String tagsID2 = "";
                    
                if (myserial.IsOpen)    
                {
                    while (!(myserial.BytesToRead > 0)) ;
                    Console.WriteLine("Start");                    
                    myserial.Read(data, 0, 1);
                    Thread.Sleep(50);
                    myserial.Read(data, 1, 12);
                    if (data[0]=='8')
                    {
                        char[] data2 = new char[12];
                        myserial.Read(data2, 0, 12);
                        for (int i = 0; i < 12; i++)
                        {
                            tagsID2 += data2[i];
                        }
                    }
                    for (int i = 0; i < 13; i++)
                    {
                        Console.Write(data[i]);
                    }
                    Console.WriteLine();
                    Console.WriteLine("End");
              
                    for (int i = 1; i < 13; i++)
                    {
                        tagsID += data[i];
                    }
                    Console.WriteLine(tagsID);
                    Console.WriteLine(checkTags(tagsID));
                    switch (data[0])
                {
                     case '1':myserial.Write(checkTags(tagsID).ToString());
                        Thread.Sleep(50);
                        break;
                    case '2':
                        myserial.Write(checkBook(tagsID).ToString());
                        Thread.Sleep(50);
                        break;
                    case '3':
                        returnBook(tagsID);
                        break;
                    case '4':                       
                        myserial.WriteLine(getName(tagsID));
                        Thread.Sleep(50);
                        break;
                    case '5':
                        myserial.WriteLine(getPIN(tagsID));
                        Thread.Sleep(50);

                        break;
                    case '6':
                        myserial.Write(getUserStatus(tagsID)[0].ToString());
                        Thread.Sleep(50);
                        break;
                    case '7':
                        myserial.Write(getUserStatus(tagsID)[1].ToString());
                        Thread.Sleep(50);
                        break;
                    case '8':
                        doBorrowBook(tagsID, tagsID2);
                        break;
                    default:
                        break;
                }
                }
                else
                {
                    Console.WriteLine("Not Connected");
                }

                
            }
        }

        private static int checkTags(String tagsID)
        {
            LibraryDemoEntities1 lar = new LibraryDemoEntities1();
            var tags = lar.RFIDTags.FirstOrDefault(a => a.UID == tagsID);
            if (tags != null)
            {
                if (tags.type==1)
                {
                    return 2;
                }
                return 1;
            }
            return 0;
        }
        private static String getName(String tagsID)
        {
            LibraryDemoEntities1 lar = new LibraryDemoEntities1();
            var tags = lar.RFIDTags.FirstOrDefault(a => a.UID == tagsID);
            if (tags.type == 1)
            {
                var book = lar.Books.FirstOrDefault(a => a.rfidTag == tags.RFIDTagID);
                return book.title;
            }
            else {
            var user = lar.Users.FirstOrDefault(a => a.rfidTag == tags.RFIDTagID);
            return user.fullName;}
        }
        private static String getPIN(String tagsID)
        {
            LibraryDemoEntities1 lar = new LibraryDemoEntities1();
            var tags = lar.RFIDTags.FirstOrDefault(a => a.UID == tagsID);
            var user = lar.Users.FirstOrDefault(a => a.rfidTag == tags.RFIDTagID);
            return user.PINNum;
        }

        private static int[] getUserStatus(String tagsID)
        {
            int[] userStatus = new int[2];
            LibraryDemoEntities1 lar = new LibraryDemoEntities1();
            var tags = lar.RFIDTags.FirstOrDefault(a => a.UID == tagsID);
            var user = lar.Users.FirstOrDefault(a => a.rfidTag == tags.RFIDTagID);
            var trans = lar.Transactions.Where(a => a.userID == user.userID && a.status == 1).ToList();
            userStatus[0] = trans.Count();
            userStatus[1] = 0;
            foreach (var item in trans)
            {
                if (item.expectedReturn<DateTime.Today)
                {
                    userStatus[1]++;
                }
            }
            return userStatus;
        }

        private static int checkBook(String tagsID)
        {
            LibraryDemoEntities1 lar = new LibraryDemoEntities1();
            var tags = lar.RFIDTags.FirstOrDefault(a => a.UID == tagsID);
            var book = lar.Books.FirstOrDefault(a=>a.rfidTag==tags.RFIDTagID);
            return book.status == 1 ? 1 : 0;
        }


        private static void returnBook(String tagsID)
        {
            LibraryDemoEntities1 lar = new LibraryDemoEntities1();
            var tags = lar.RFIDTags.FirstOrDefault(a => a.UID == tagsID);
            var book = lar.Books.FirstOrDefault(a => a.rfidTag == tags.RFIDTagID);
            var trans = lar.Transactions.FirstOrDefault(a => a.status == 1 && a.bookID == book.bookID);
            trans.status = 2;
            book.status = 1;
            lar.SaveChanges();
        }

        private static void doBorrowBook(String userID, String bookID)
        {
            System.TimeSpan t = new TimeSpan(72, 0, 0);
            Transaction trans = new Transaction();
            LibraryDemoEntities1 lar = new LibraryDemoEntities1();
            var tags = lar.RFIDTags.FirstOrDefault(a => a.UID == userID);
            var user = lar.Users.FirstOrDefault(a => a.rfidTag == tags.RFIDTagID);
            tags = lar.RFIDTags.FirstOrDefault(a => a.UID == bookID);
            var book = lar.Books.FirstOrDefault(a => a.rfidTag == tags.RFIDTagID);
            trans.status = 1;
            trans.userID = user.userID;
            trans.bookID = book.bookID;
            trans.borrowDate =DateTime.Now;
            trans.expectedReturn = DateTime.Now+t;
            book.status = 2;
            lar.Transactions.Add(trans);
            lar.SaveChanges();
        }
    }
    
        
}
