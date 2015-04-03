using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace Library
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.PortName = "COM11";
            serialPort1.BaudRate = 9600;
            try
            {
                serialPort1.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. Check again!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
                
            while (true)
            {
                try
                {
                    string data_rx = serialPort1.ReadLine();
                    var name = getName(data_rx);
                    if (name != null)
                    {
                        lblName.Text = name;
                    }
                    else
                    {
                        MessageBox.Show("Khong thay ten");
                    }
                }
                catch
                {

                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private string getName(string uid)
        {
            
            try
            {
                var dbContext = new LibraryDemoEntities();
                var name = dbContext.RFIDTags.FirstOrDefault(m => m.UID.Equals(uid)).Users.First().fullName;
                return name;
            }
            catch
            {
                return null;
            }
        }   

        private string getBookName(string uid)
        {
            try
            {
                var dbContext = new LibraryDemoEntities();
                var book = dbContext.RFIDTags.FirstOrDefault(m => m.UID.Equals(uid)).Books.First().title;
                return book;
            }
            catch 
            {
                return null;
            }
        }

        private string addTrans(int userid, int bookid)
        {
            try
            {
                var dbContext = new LibraryDemoEntities();
                var trans = new Transaction();
                trans.userID = userid;
                trans.bookID = bookid;
                dbContext.Transactions.Add(trans);
                dbContext.SaveChanges();
            }
            catch (Exception)
            {
                MessageBox.Show("Error. Check again!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
            
        }

        private string changeStatusBook(int bookid, string newStatus)
        {
            try
            {
                var dbContext = new LibraryDemoEntities();
                var book = dbContext.Books.FirstOrDefault(b => b.bookID == bookid);
                var bstatus = dbContext.BookStatus.FirstOrDefault(s => s.title == newStatus);
                book.status = bstatus.bookStatusID;
                dbContext.SaveChanges();
            }
            catch (Exception)
            {
                MessageBox.Show("Error. Check again", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
                return null;
            
        }

        private string doBorrowBook(int userid, int bookid)
        {
            try
            {
                addTrans(userid, bookid);
                changeStatusBook(bookid, "Dang Muon");
            }
            catch (Exception)
            {
                MessageBox.Show("Error. Check again", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        private string doReturnBook(int userid, int bookid)
        {
            try
            {
                addTrans(userid, bookid);
                changeStatusBook(bookid, "Da Tra");
            }
            catch (Exception)
            {
                MessageBox.Show("Error. Please check again!","Message",MessageBoxButtons.OK,MessageBoxIcon.Error);  
            }
            return null;
        }

        private string getStatusBook(int bookid)
        {
            var dbContext = new LibraryDemoEntities();
           // var bstatus = dbContext.Books.FirstOrDefault(s => s.)
            return null;
        }

        private string getListBook(int bookid)
        {
            var dbContext = new LibraryDemoEntities();
         //   var book 
            return null;
        }

        public void filter()
        {
            var dbContext = new LibraryDemoEntities();
            IQueryable<Book> query = dbContext.Books;
            query.Where(p => p.status == 1); // status cua book 1: Dang Muon
            List<Book> filterBooks = query.ToList();
        }
    }
}
