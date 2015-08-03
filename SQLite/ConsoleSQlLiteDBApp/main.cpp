// SQLTest.cpp : Defines the entry point for the application.
//
#include "stdafx.h"
#include <stdio.h>								
#include "sqlite3.h"
#include <string.h>

//#define MAX_PATH 255
//#define DWORD uint32

#define DBNAME "C:\\Temp\\MyDB"

// SQLite DBMS Syntax: http://www.sqlite.org/lang.html
// SQLite C++ API: http://www.sqlite.org/c3ref/intro.html

// Callback to make use of returned queries with sqlite_exec3
static	int	callback(void	*NotUsed, int	argc, char	**argv, char	**azColName) {
	int	i;
	for (i = 0; i<argc; i++) {
		////Display column name and value for each record
		//printf("%s	=	%s\r\n", azColName[i], argv[i] ? argv[i] : NULL);
		//wprintf(L"\r\n");

		//Some Unicode conversions so as to use RETAILMSG() as well
		TCHAR colStr[MAX_PATH + 1];
		TCHAR valStr[MAX_PATH + 1];
		valStr[0] = (TCHAR)0;
		MultiByteToWideChar(CP_UTF8, 0, azColName[i], -1, colStr, MAX_PATH + 1);
		if (argv[i] != NULL)
			MultiByteToWideChar(CP_UTF8, 0, argv[i], -1, valStr, MAX_PATH + 1);
		wprintf( L"%s =	%s\t", colStr, valStr);
	}
	wprintf(L"\r\n");
	return	0;
}

bool FileExists(const TCHAR *fileName)
{
	DWORD       fileAttr;

	fileAttr = GetFileAttributes(fileName);
	if (0xFFFFFFFF == fileAttr)
		return false;
	return true;
}

void  ConvertToChar(LPCWSTR inStr, char * outStr)
{
	WideCharToMultiByte(CP_UTF8, 0, inStr, -1, outStr, sizeof(outStr),
		NULL, NULL);
}

// Creat the sqlite temp directory
static void sqlite_init()
{
	// Note to Windows users(1): 
	// From http://www.sqlite.org/c3ref/open.html
	// The encoding used for the filename argument of sqlite3_open() and sqlite3_open_v2() must be UTF-8, 
	//  not whatever codepage is currently defined. 
	// Filenames containing international characters must be converted to UTF-8 prior to passing them into 
	// sqlite3_open() or sqlite3_open_v2().

	// Note to Windows Runtime users (2): 
	// From: http://www.sqlite.org/c3ref/temp_directory.html (and above link)
	// The temporary directory must be set prior to calling sqlite3_open or sqlite3_open_v2. 
	// Otherwise, various features that require the use of temporary files may fail. 
	// Here is an example of how to do this using C++ with the Windows Runtime:  (Modified for Compact):


	LPCWSTR zPath = L"c:\\Temp\\Sqlite";
	//Create the sqlite temp dir directory if it doesn't exist
	//Assume if it does exist then the temp dir has been set already
	if (!FileExists(zPath))
	{
		char  zPathBuf[MAX_PATH + 1];
		ConvertToChar(zPath, zPathBuf);
		sqlite3_temp_directory = sqlite3_mprintf("%s", zPathBuf);
		wprintf(L"Set sqlite temp directory\r\n");
	}

}




//int	mainx(int	argc, char	**argv) {
//	sqlite3	*db;
//	char	*zErrMsg = 0;
//	int	rc;
//
//	puts(argv[0]);
//	puts(argv[1]);
//	printf(" asd %d \n", argc);
//
//
//	if (argc != 3) {
//		fprintf(stderr, "xUsage: %s DATABASE SQL-STATEMENT\n", argv[0]);
//		return(1);
//	}
//	rc = sqlite3_open(argv[1], &db);
//	if (rc) {
//		fprintf(stderr, "Can't open database: %s\n", sqlite3_errmsg(db));
//		sqlite3_close(db);
//		return(1);
//	}
//	rc = sqlite3_exec(db, argv[2], callback, 0, &zErrMsg);
//	if (rc != SQLITE_OK) {
//		fprintf(stderr, "Main SQL error: %s\n", zErrMsg);
//		sqlite3_free(zErrMsg);
//	}
//
//	sqlite3_close(db);
//
//	return	0;
//}

int main(int argc, char* argv[])
{
	sqlite3 *db;
	char *zErrMsg = 0;
	int rc;

	sqlite_init();

	LPCWSTR dbName = _T(DBNAME);
	char dbNameChar[] = DBNAME;

	//Delete the db if it exists
	if (FileExists(dbName))
	{
		wprintf(L"DB File exists so deleting it.\r\n");
		DeleteFile(dbName);
	}

	rc = sqlite3_open(dbNameChar, &db);
	if (rc) {
		fprintf(stderr, "Can't open database: %s\n", sqlite3_errmsg(db));
		sqlite3_close(db);
		return 1;
	}

	wprintf(L"Openned the DB\r\n");

	rc = sqlite3_exec(db, "CREATE TABLE tblStuff ( id INTEGER PRIMARY KEY , name, value INTEGER )", callback, 0, &zErrMsg);
	wprintf(L"Created a table\r\n");

	//Hint: Insert NULL for id to autoincrement as its INTEGER PRIMARY KEY
	rc = sqlite3_exec(db, "INSERT INTO tblStuff Values (NULL,'Temperature1',23)", callback, 0, &zErrMsg);
	rc = sqlite3_exec(db, "INSERT INTO tblStuff Values (NULL,'Temperature2',67)", callback, 0, &zErrMsg);
	rc = sqlite3_exec(db, "INSERT INTO tblStuff Values (NULL,'Hunidity1',98)", callback, 0, &zErrMsg);
	wprintf(L"Added some records:\r\n");
	rc = sqlite3_exec(db, "SELECT * FROM tblStuff", callback, 0, &zErrMsg);

	if (rc != SQLITE_OK) {
		fprintf(stderr, "SQL error: %s\n", zErrMsg);
		sqlite3_free(zErrMsg);
	}
	sqlite3_close(db);
	wprintf(L"Closed the DB\r\n");

	//Reopen the db
	rc = sqlite3_open(dbNameChar, &db);
	if (rc) {
		fprintf(stderr, "Can't open database: %s\n", sqlite3_errmsg(db));
		sqlite3_close(db);
		return 1;
	}

	wprintf(L"Reopenned the DB\r\n");

	wprintf(L"Records are still there?:\r\n");
	rc = sqlite3_exec(db, "SELECT * FROM tblStuff", callback, 0, &zErrMsg);

	rc = sqlite3_exec(db, "INSERT INTO tblStuff Values (NULL,'Temperature1',79)", callback, 0, &zErrMsg);
	rc = sqlite3_exec(db, "INSERT INTO tblStuff Values (NULL,'Temperature2',88)", callback, 0, &zErrMsg);
	rc = sqlite3_exec(db, "INSERT INTO tblStuff Values (NULL,'Hunidity1',70)", callback, 0, &zErrMsg);
	wprintf(L"Added some more records:\r\n");
	rc = sqlite3_exec(db, "SELECT * FROM tblStuff", callback, 0, &zErrMsg);

	rc = sqlite3_exec(db, "UPDATE tblStuff SET name = 'Temperature10' WHERE name = 'Temperature1'", callback, 0, &zErrMsg);
	rc = sqlite3_exec(db, "DELETE FROM tblStuff WHERE name = 'Humidity1'", callback, 0, &zErrMsg);
	wprintf(L"Updated and deleted some records:\r\n");
	rc = sqlite3_exec(db, "SELECT * FROM tblStuff", callback, 0, &zErrMsg);

	rc = sqlite3_exec(db, "DROP TABLE tblStuff", callback, 0, &zErrMsg);
	wprintf(L"Dropped the table\r\n");

	rc = sqlite3_exec(db, "DROP TABLE tblStuff", callback, 0, &zErrMsg);

	wprintf(L"Dropped the table\r\n");

	if (rc != SQLITE_OK) {
		fprintf(stderr, "SQL error: %s\n", zErrMsg);
		sqlite3_free(zErrMsg);
	}
	sqlite3_close(db);

	wprintf(L"Closed the DB\r\n");

	return 0;
}







