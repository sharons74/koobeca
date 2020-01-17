Running the DAL on the local machine
---------------------------------------------------

1. Install XAMPP - Download and install XAMPP from https://www.apachefriends.org/index.html
2. Run the XAMPP control panel from the windows start menu (https://imgur.com/bnT0ZYK)
3. Start the MySql subsystem (before: https://imgur.com/1gBmQh6 , after: https://imgur.com/fmrgrUi)
4. Open a XAMPP shell (https://imgur.com/vg2w3Qh)
5. Download the mysql backup file from koobeca.com (https://imgur.com/YD1sSjx)
	5.1. ftp koobeca.com
	5.2. Username: koobadmin
	5.3. Password: .... (clock...)
	5.4. ftp> bin
	5.5. ftp> cd sqlbackups
	5.6. ftp> get backup-2018-12-08.sql
	5.7. ftp> quit
6. Restore the sql backup into the local MySql server (https://imgur.com/9jRbckG)
	6.1 mysql -u root -p koobdb < backup-2018-12-08.sql
	  Note: if this is the default XAMPP installation, just press enter when prompted for a password,
	        otherwise, if you've set a password for mysql in the past, then you should enter it at
			the prompt.
7. Test that the koobdb database was successfully restored (https://imgur.com/xCMi39A)
   7.1 # mysql -u root -p koobdb
   7.2 MariaDB [koobdb]> show tables;
8. You are now ready to run the unit tests in visual studio, it will connect to the local MySql and run
     all SQL queries against it.

Configuring the C# code
---------------------------------------------------
The connection settings are hard coded in the file DAL\DbConnection.cs at this line:

21: var connstring = $"Server=localhost; database={DatabaseName}; UID=root; password=";

If you have a mysql password set it there.
