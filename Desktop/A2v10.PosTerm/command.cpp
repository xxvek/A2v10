// Copyright � 2015-2020 Alex Kukhtin. All rights reserved.

#include "pch.h"
#include "posterm.h"
#include "commanddata.h"
#include "command.h"
#include "fiscalprinter.h"
#include "fiscalprinterImpl.h"


const size_t CMD_MAX_LEN = 64;

//static 
PosCommand::COMMAND_BIND PosCommand::_binded_commands[] = 
{
	{L"nullReceipt",  &PosCommand::NullReceipt, &PosCommand::NullReceiptData},
	{L"xReport",      &PosCommand::XReport,     nullptr},
	{L"zReport",      &PosCommand::ZReport,     nullptr},
	{L"printReceipt", &PosCommand::PrintReceipt, &PosCommand::PrintReceiptData},
	{nullptr, nullptr}
};

PosCommand::PosCommand()
{
}

pos_result_t PosCommand::ExecuteCommand(std::wstring& result)
{
	result.clear();
	FiscalPrinter* pPrinter = FiscalPrinter::FindPrinter(_id.c_str());
	if (pPrinter == nullptr)
		return pos_result_t::_device_not_found;
	return ExecuteCommandInt(pPrinter, result);
}

pos_result_t PosCommand::ExecuteCommandInt(FiscalPrinter* pPrinter, std::wstring& result)
{
	PosCommand::COMMAND_BIND* entry = _binded_commands;
	while (entry && entry->_name) {
		if (_command == entry->_name) {
			auto func = entry->_func;
			try {
				(this->*entry->_func)(pPrinter, _data.get(), result);
				return pos_result_t::_success;
			}
			catch (CFPException ex) {
				result = ex.GetError();
				return pos_result_t::_generic_error;
			}
		}
		entry++;
	}
	return pos_result_t::_success;
}

// virtual 
JsonTarget* PosCommand::CreateObject(const wchar_t* szName) 
{
	if (wcsncmp(szName, L"data", CMD_MAX_LEN) == 0) {
		PosCommand::COMMAND_BIND* entry = _binded_commands;
		while (entry && entry->_name) {
			if (_command == entry->_name) {
				auto func = entry->_createData;
				if (func != nullptr) {
					JsonTarget* pData = (this->*entry->_createData)();
					_data.reset(pData);
					return _data.get();
				}
			}
			entry++;
		}
	}
	return _data.get();
}

void PosCommand::NullReceipt(FiscalPrinter* pPrinter, JsonTarget* data, std::wstring& result)
{
	PosNullReceiptData* pnrd = dynamic_cast<PosNullReceiptData*>(data);
	bool bOpenDrawer = false;
	if (pnrd)
		bOpenDrawer = pnrd->m_openCashDrawer;
	pPrinter->NullReceipt(bOpenDrawer);
}

JsonTarget* PosCommand::NullReceiptData()
{
	return new PosNullReceiptData();
}


void PosCommand::XReport(FiscalPrinter* pPrinter, JsonTarget* data, std::wstring& result)
{
	pPrinter->XReport();
}

void PosCommand::ZReport(FiscalPrinter* pPrinter, JsonTarget* data, std::wstring& result)
{
	pPrinter->ZReport();
}

JsonTarget* PosCommand::PrintReceiptData()
{
	return new PosPrintReceiptData();
}

void PosCommand::PrintReceipt(FiscalPrinter* pPrinter, JsonTarget* data, std::wstring& result)
{
	PosPrintReceiptData* pprd = dynamic_cast<PosPrintReceiptData*>(data);
	pPrinter->PrintReceipt(pprd);
	result.assign(L"{\"no\": 123}");
}


