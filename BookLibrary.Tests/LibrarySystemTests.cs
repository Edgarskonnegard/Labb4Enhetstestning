namespace Labb4Enhetstestning.BookLibrary.Tests;
using Labb4Enhetstestning.BookLibrary;

[TestClass]
public class LibrarySystemTests
{
    //Testing Add book method.
    [TestMethod]
    public void AddBook_DuplicateISBN_False()
    {
        var library = new LibrarySystem();
        var result = library.AddBook(new Book("Min resa", "Kalle Anka", "9780451524935", 1936));
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void AddBook_NoISBN_False()
    {
        var library = new LibrarySystem();
        var result = library.AddBook(new Book("Min resa", "Kalle Anka", "", 1936));
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void AddBook_ISBNIncludesNonDigits_False()
    {
        var library = new LibrarySystem();
        var result = library.AddBook(new Book("Min resa", "Kalle Anka", "1223length111", 1936));
        Assert.IsFalse(result);
    }

    //Testing Remove book method.
    [TestMethod]
    public void RemoveBook_ValidISBN_True()
    {
        var library = new LibrarySystem();
        var result = library.RemoveBook("9780451524935");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void RemoveBook_InvalidISBN_False()
    {
        var library = new LibrarySystem();
        var result = library.RemoveBook("1110451524935");
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void RemoveBook_RemoveBorrowedBook_False()
    {
        var library = new LibrarySystem();
        library.BorrowBook("9780451524935");
        var result = library.RemoveBook("9780451524935");
        Assert.IsFalse(result);
    }

    //Test search method
    [TestMethod]
    public void SearchByAuthor_CapitalLetters_True()
    {
        var library = new LibrarySystem();
        var result = library.SearchByAuthor("GEORGE ORWELL");
        var expected = library.SearchByAuthor("George Orwell");
        CollectionAssert.AreEqual(result, expected);
    }

    [TestMethod]
    public void SearchByAuthor_PartMatching_True()
    {
        var library = new LibrarySystem();
        var result = library.SearchByAuthor("George");
        Assert.IsTrue(result.Any(b => b.Author.Equals("George Orwell", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void SearchByTitle_CapitalLetters_True()
    {
        var library = new LibrarySystem();
        var result = library.SearchByTitle("THE HOBBIT");
        Assert.IsTrue(result.Any(b => b.Title.Contains("The Hobbit")));
    }

    [TestMethod]
    public void SearchByTitle_PartMatching_True()
    {
        var library = new LibrarySystem();
        var result = library.SearchByTitle("Th");
        Assert.IsTrue(result.Any(b => b.Title.Contains("The Hobbit",StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void SearchByISBN_PartMatching_True()
    {
        var library = new LibrarySystem();
        var result = library.SearchByISBN("9780");
        var expected = library.SearchByISBN("9780451524935");
        Assert.AreEqual(result, expected);
    }
    


    //Testing borrow method
    [TestMethod]
    public void BorrowBook_MarkedAsBorrowed_True()
    {
        var library = new LibrarySystem();
        library.BorrowBook("9780451524935");
        var result = library.SearchByISBN("9780451524935").IsBorrowed;
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void BorrowBook_BorrowAlreadyBorrowedBook_False()
    {
        var library = new LibrarySystem();
        library.BorrowBook("9780451524935");
        var result = library.BorrowBook("9780451524935");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void BorrowBook_CorrectDate_True()
    {
        var library = new LibrarySystem();
        library.ReturnBook("9780451524935");
        library.BorrowBook("9780451524935");
        var expected = DateTime.Now;
        var result = library.SearchByISBN("9780451524935").BorrowDate;
        Assert.AreEqual(expected.Date, result.Value.Date);
    }

    //Test return method
    [TestMethod]
    public void ReturnBook_ResetBorrowDate_True()
    {
        var library = new LibrarySystem();
        library.BorrowBook("9780451524935");
        library.ReturnBook("9780451524935");
        var result = library.SearchByISBN("9780451524935");
        Assert.IsNull(result.BorrowDate);
    }

    [TestMethod]
    public void ReturnBook_ReturnUnborrowedBook_False()
    {
        var library = new LibrarySystem();
        var result = library.ReturnBook("9780451524935");
        Assert.IsFalse(result);
    }

    //Late return of book test
    [TestMethod]
    public void IsBookOverdue_OverdueBook_True()
    {
        var library = new LibrarySystem();
        var book = library.SearchByISBN("9780451524935");
        book.BorrowDate = DateTime.Now.AddDays(-30);
        book.IsBorrowed = true;
        var result = library.IsBookOverdue("9780451524935",15); 
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CalculateLateFee_LateByThreeDays_True()
    {
        var library = new LibrarySystem();
        var book = library.SearchByISBN("9780451524935");
        book.BorrowDate = DateTime.Now.AddDays(-15);
        book.IsBorrowed = true;
        var result = library.CalculateLateFee("9780451524935", 3); 
        var expected = 0.5m*3;
        Assert.AreEqual(result, expected);
    }
}