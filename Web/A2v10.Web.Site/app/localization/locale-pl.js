﻿// Copyright © 2015-2024 Oleksandr Kukhtin. All rights reserved.

// 20240118-7967
// locale-pl.js

"use strict";

(function () {

	window.$$locale = {
		$Locale: 'pl-PL',
		$DateLocale: 'pl-PL',
		$NumberLocale: 'pl-PL',
		$Ok: 'OK',
		$Cancel: 'Anuluj',
		$Close: 'Zamknij',
		$Apply: 'Zastosuj',
		$Back: 'Cofnij',
		$Next: 'Dalej',
		$Finish: 'Gotowy',
		$Tasks: 'Zadania',
		$Quit: 'Wylogowanie się',
		$Save: 'Zapisz',
		$NotSave: 'Nie zapisuj',
		$Refresh: 'Aktualizacja',
		$Confirm: 'Potwierdzenie',
		$Message: 'Wiadomość',
		$Error: 'Błąd',
		$Help: 'Pomoc',
		$ConfirmClose: 'Potwierdzenie zamknięcia',
		$MakeValidFirst: 'Najpierw napraw błędy',
		$ElementWasChanged: 'Przedmiot został zmieniony. Zapisać zmiany?',
		$Profiling: 'Profilowanie',
		$DataModel: 'Model danych',
		$Admin: 'administrator',
		$Today: 'Dziś',
		$Yesterday: 'Wczoraj',
		$Week: 'Tydzień',
		$Month: 'Miesiąc',
		$CreateLC: 'Stwórz',
		$NoElements: 'brak elementów',
		$PagerElements: 'elementy',
		$Of: 'z',
		$Register: 'Rejestracja',
		$ClickToDownloadPicture: 'Kliknij, aby przesłać obraz',
		$ClickToDownloadFile: 'Kliknij, aby pobrać plik',
		$EnterPassword: 'Wprowadź hasło',
		$MatchError: 'Hasło nie zgadza się z potwierdzeneim',
		$PasswordLength: 'Hasło musi mieć co najmniej 6 znaków',
		$PasswordAgain: 'Potwierdzenie',
		$InvalidOldPassword: 'Błędne stare hasło',
		$ChangePasswordNotAllowed: 'Zmiana hasła zabroniona',
		$ChangePasswordSuccess: 'Hasło zmieniono pomyślnie',
		$ChangePassword: 'Zmień hasło',
		$Last7Days: 'Ostatnie 7 dni',
		$Last30Days: 'Ostatnie 30 dni',
		$MonthToDate: 'Od początku miesiąca',
		$PrevMonth: 'Poprzedni miesiac',
		$CurrMonth: 'Obecny miesiąc',
		$QuartToDate: 'Od początku kwartału',
		$PrevQuart: 'Poprzedni kwartał',
		$CurrQuart: 'Bieżący kwartał',
		$YearToDate: 'Od początku roku',
		$AllPeriodData: 'Przez cały okres',
		$CurrYear: 'W tym roku',
		$PrevYear: 'Ostatni rok',
		$CustomPeriod: 'Dowolnie',
		$Hours: 'Zegar',
		$Minutes: 'Minuty',
		$License: 'licencja',
		$HomePage: 'Strona główna',
		$CreatedOpenSource: 'Stworzony przy użyciu oprogramowania open source',
		$Unknown: 'Nie określono',
		$ChooseFile: 'Wybierz plik',
		$AccessDenied: 'Odmowa dostępu do systemu!',
		$PermissionDenied: 'Odmowa dostępu!',
		$FileTooLarge: 'Plik jest za duży. Rozmiar pliku nie może przekraczać {0} KB',
		$DesktopNotSupported: 'Ta czynność nie jest obsługiwana w wersji desktopowej',
		$Settings: 'Ustawienie',
		$Feedback: 'Informacja zwrotna',
		$PreviewIsUnavailable: 'Preview is unavailable for this file',
		$ShowSpecProps: 'Show special properties'
	};

	if (window.d3) {
		d3.formatDefaultLocale({
			decimal: ",",
			thousands: "\xa0",
			grouping: [3],
			currency: ["'zł", ""]
		});

		d3.timeFormatDefaultLocale({
			"dateTime": "%A, %e %B %Y г. %X",
			"date": "%d.%m.%Y",
			"time": "%H:%M:%S",
			"periods": ['przed', 'po południu'],
			"days": ['niedziela', 'poniedziałek', 'wtorek', 'środa', 'czwartek', 'piątek', 'sobota'],
			"shortDays": ['Nd', 'Pon', 'Wt', 'Śr', 'Czw', 'Pt', 'Sob'],
			"months":  ['stycznia', 'lutego', 'marca', 'kwietnia', 'maja', 'czerwca', 'lipca', 'sierpnia', 'września', 'października', 'listopada', 'grudnia'],
			"shortMonths":  ['st', 'lut', 'mrz', 'kw', 'maj', 'cz', 'lip', 'sier', 'wrz', 'paź', 'lis', 'gr' ]
		});
	}
})();