// ConsoleApplication1.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <utility>
#include <memory>
using std::unique_ptr;

int _tmain(int argc, _TCHAR* argv[])
{
//    unique_ptr<TCHAR> my(L"This is my Unique ptr");
//    unique_ptr<TCHAR> my1 (new TCHAR[128]);
//    TCHAR *pMy1 = my1.get();
//    _tcscpy(pMy1, L"This is updated values");
//    my.reset();
//    my1.reset();
    const TCHAR s3[] (L"This is buffer");
    TCHAR  simpleStr[128] = L"";
    wchar_t s1[128] = L"String S1";
    TCHAR s2[] = __T("This is my string");
    int count = _tcslen(simpleStr);
    count = _countof(simpleStr);
    wchar_t *pS3 = new wchar_t[30];
    _tcscpy_s(pS3, _tcslen(s3) , s3);
    count = _tcslen(s1);
    count = _tcslen(__T("This is my string"));
    _tcscpy_s(simpleStr, s2);
    _tcscpy_s(simpleStr, s1);
    delete pS3;
    pS3 = nullptr;
	return 0;
}

