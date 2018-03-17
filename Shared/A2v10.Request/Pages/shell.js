﻿
/*20180317-7132*/

(function () {

	const Shell = component('std:shellController');

	const menu = $(Menu);

	const shell = new Shell({
		el: '#shell',
		data: {
			version: '$(AppVersion)',
			menu: menu.Menu ? menu.Menu[0].Menu : null,
			title: menu.SysParams.AppTitle,
			subtitle: menu.SysParams.AppSubTitle,
			sideBarMode: menu.SysParams.SideBarMode,
			userIsAdmin: $(Admin)
		}
	});

	window.$rootUrl = '';

})();