﻿/*20170813-7005*/
/*shell.js*/

/*
TODO:
3. SideBar - разные режимы
4. SideBar - дерево
*/

/* routing rules 

1. /top
2. /top/side
3. /top/action/id
4. /top/side/action/id

*/

(function () {

    const bus = require('eventBus');
    let menu = [
        { title: "Home", url: "home" },
        {
            title: 'Catalog', url: 'catalog', menu: [
                { title: "Suppliers", url: 'suppliers' },
                { title: "Customers", url: 'customers' },
                { title: "Edit 3 segment", url: 'edit/5' }
            ]
        },
        {
            title: 'Document', url: 'document', menu: [
                { title: "Incoming", url: 'incoming' },
                { title: "Outgoing", url: 'outgoing' },
                { title: "edit 4 segment", url: 'outgoing/edit/2' }
            ]
        }
    ];

    function Location() {
        this.wl = window.location.pathname.split('/');
    }

    Location.prototype.routeLength = function () {
        return this.wl.length;
    };

    Location.prototype.segment = function (no) {
        let wl = this.wl;
        return wl.length > no ? wl[no].toLowerCase() : '';
    };

    Location.prototype.saveMenuUrl = function () {
        let s1 = this.segment(1);
        let s2 = this.segment(2);
        if (s1) {
            let key = 'menu:' + s1;
            if (s2)
                window.localStorage.setItem(key, s2);
            else
                window.localStorage.removeItem(key);
        }
    };

    const navBar = {
        props: {
            menu: Array
        },
        template: '<ul class="nav-bar"><li v-for="item in menu" :key="item.url" ><a href="\" :class="{active : isActive(item)}" v-text="item.title" @click.stop.prevent="navigate(item)"></a></li></ul>',

        data: function () {
            return {
                activeItem: null
            };
        },

        created: function () {
            var me = this;

            function findCurrent() {
                let loc = new Location();
                let seg1 = loc.segment(1);
                me.activeItem = me.menu.find(itm => itm.url === seg1);
                return loc;
            }

            window.addEventListener('popstate', function (event, a, b) {
                findCurrent().saveMenuUrl();
                bus.$emit('route');
            });

            findCurrent();

            if (!me.activeItem) {
                me.activeItem = me.menu[0];
                window.history.replaceState(null, me.activeItem.title, me.activeItem.url);
            }
        },

        methods: {
            isActive: function (item) {
                return item === this.activeItem;
            },
            navigate: function (item) {
                let key = `menu:${item.url}`;
                this.activeItem = item;
                let url = '/' + item.url;
                let savedUrl = window.localStorage.getItem(key);
                if (savedUrl) {
                    url = url + '/' + savedUrl;
                } else {
                    // todo: find active item
                    if (item.menu && item.menu.length) {
                        url = url + '/' + item.menu[0].url;
                    }
                }
                window.history.pushState(null, null, url);
                bus.$emit('route');
            }
        }
    };

    const sideBar = {
        // TODO: разные варианты меню
        template: '<ul class="side-menu"><li v-for="itm in sideMenu" :key="itm.url" :class="{active: isActive(itm)}"><a href="" v-text="itm.title" @click.stop.prevent="navigate(itm)"></a></li></ul>',
        props: {
            menu: Array
        },
        data: function () {
            return {
                sideMenu: null,
                topUrl: null,
                activeItem: null
            };
        },
        methods: {
            isActive: function (itm) {
                return itm === this.activeItem;
            },
            navigate: function (itm) {
                let newUrl = `/${this.topUrl}/${itm.url}`;
                window.history.pushState(null, itm.title, newUrl);
                new Location().saveMenuUrl();
                bus.$emit('route');
            }
        },
        created: function () {
            var me = this;
            bus.$on('route', function () {
                let loc = new Location();
                let s1 = loc.segment(1);
                let s2 = loc.segment(2);
                let m1 = me.menu.find(itm => itm.url === s1);
                if (!m1) {
                    me.topUrl = null;
                    me.sideMenu = null;
                } else {
                    me.topUrl = m1.url;
                    me.sideMenu = m1.menu || null;
                    if (me.sideMenu)
                        me.activeItem = me.sideMenu.find(itm => itm.url === s2);
                }
            });
        }
    };

    const contentView = {
        render(h) {
            return h('div', {
                attrs: {
                    class: 'content-view ' + this.cssClass
                }
            }, [h('include', {
                props: {
                src: this.currentView
            }})]);
        },
        data() {
            return {
                currentView: null,
                cssClass: ''
            };
        },
        created() {
            var me = this;
            bus.$on('route', function () {
                let loc = new Location();
                //TODO: += query
                me.currentView = "/_page" + window.location.pathname;
                let len = loc.routeLength();
                me.cssClass =
                    len === 2 ? 'full-page' :
                    len === 3 ? 'partial-page' :
                    'full-view';
            });
        }
    };

    // important: use v-show instead v-if to ensure components created only once
    const a2MainView = {
        template: `
<div class="main-view">
    <a2-nav-bar :menu="menu" v-show="navBarVisible"></a2-nav-bar>
    <a2-side-bar :menu="menu" v-show="sideBarVisible"></a2-side-bar>
    <a2-content-view></a2-content-view>
    <div class='modal-wrapper' v-if="hasModals">
        <div class="modal-window" tabindex="0" v-for="dlg in modals" @keyup.esc='closeModal'>
            <span>{{dlg.title}} {{dlg.url}}</span><button @click.stop='closeModal'>x</button>
            <include :src="dlg.url"></include>
        </div>
    </div>
</div>`,
        components: {
            'a2-nav-bar': navBar,
            'a2-side-bar': sideBar,
            'a2-content-view': contentView
        },
        props: {
            menu: Array
        },
        data() {
            return {
                navBarVisible: false,
                sideBarVisible: true,
                modals: []
            };
        },
        computed: {
            hasModals: function () {
                return this.modals.length > 0;
            }
        },        
        mounted() {
            // first time created
            new Location().saveMenuUrl();
            bus.$emit('route');
        },
        methods: {
            closeModal() {
                this.modals.pop();
            },
            keyUp() {
                alert('key up');
            }
        },
        created() {
            let me = this;
            bus.$on('route', function () {
                let loc = new Location();
                let len = loc.routeLength();
                let seg1 = loc.segment(1);
                me.navBarVisible = len === 2 || len === 3;
                me.sideBarVisible = len === 3;
                me.modals.splice(0, me.modals.length);
            });
            bus.$on('modal', function (modal) {
                alert('modal event handled: ' + JSON.stringify(modal));
                me.modals.push({ title: "dialog", url: "/_page/catalog/suppliers" });
            });
        }
    };

    new Vue({
        el: '#shell',
        components: {
            'a2-main-view': a2MainView
        },
        data: {
            title: 'application title',
            menu: menu
        }
    });
})();