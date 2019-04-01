using System;
using System.Collections.Generic;
using ConnectApp.canvas;
using ConnectApp.components;
using ConnectApp.constants;
using ConnectApp.models;
using ConnectApp.redux;
using ConnectApp.redux.actions;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace ConnectApp.screens {
    public class MyEventsScreen : StatefulWidget {
        public MyEventsScreen(
            Key key = null
        ) : base(key) {
        }

        public override State createState() {
            return new _MyEventsScreenState();
        }
    }

    internal class _MyEventsScreenState : State<MyEventsScreen> {
        private PageController _pageController;
        private int _selectedIndex;

        public override void initState() {
            base.initState();
            _pageController = new PageController();
            _selectedIndex = 0;
            if (StoreProvider.store.state.mineState.futureEventsList.Count == 0)
                StoreProvider.store.Dispatch(new FetchMyFutureEventsAction {pageNumber = 0});
        }

        private static void _fetchMyPastEvents() {
            if (StoreProvider.store.state.mineState.pastEventsList.Count == 0)
                StoreProvider.store.Dispatch(new FetchMyPastEventsAction {pageNumber = 0});
        }

        public override Widget build(BuildContext context) {
            return new Container(
                color: CColors.White,
                child: new SafeArea(
                    child: new Container(
                        color: CColors.White,
                        child: new Column(
                            children: new List<Widget> {
                                _buildNavigationBar(context),
                                _buildSelectView(),
                                _buildContentView()
                            }
                        )
                    )
                )
            );
        }

        private static Widget _buildNavigationBar(BuildContext context) {
            return new Container(
                decoration: new BoxDecoration(CColors.White),
                width: MediaQuery.of(context).size.width,
                height: 140,
                child: new Column(
                    mainAxisAlignment: MainAxisAlignment.end,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: new List<Widget> {
                        new Container(
                            child:new CustomButton(
                                padding: EdgeInsets.only(16, 10, 16),
                                onPressed: () => StoreProvider.store.Dispatch(new MainNavigatorPopAction()),
                                child: new Icon(
                                    Icons.arrow_back,
                                    size: 24,
                                    color: CColors.icon3
                                )
                            ),
                            height:44
                        ),
                        new Container(
                            margin: EdgeInsets.only(16, bottom: 12),
                            child: new Text(
                                "我的活动",
                                style: CTextStyle.H2
                            )
                        )
                    }
                )
            );
        }

        private Widget _buildSelectView() {
            return new Container(
                decoration: new BoxDecoration(
                    border: new Border(
                        bottom: new BorderSide(
                            CColors.Separator2
                        )
                    )
                ),
                child: new CustomSegmentedControl(
                    new List<string> {"即将开始", "往期活动"},
                    index => {
                        if (_selectedIndex != index) {
                            if (index == 1) _fetchMyPastEvents();
                            setState(() => _selectedIndex = index);
                            _pageController.animateToPage(
                                index,
                                new TimeSpan(0, 0, 0, 0, 250),
                                Curves.ease
                            );
                        }
                    },
                    _selectedIndex
                )
            );
        }

        private Widget _buildContentView() {
            return new Flexible(
                child: new Container(
                    child: new PageView(
                        physics: new BouncingScrollPhysics(),
                        controller: _pageController,
                        onPageChanged: index => {
                            if (index == 1) _fetchMyPastEvents();
                            setState(() => { _selectedIndex = index; });
                        },
                        children: new List<Widget> {
                            _buildMyEventContent(0),
                            _buildMyEventContent(1)
                        }
                    )
                )
            );
        }

        private static Widget _buildMyEventContent(int index) {
            return new Container(
                child: new StoreConnector<AppState, MineState>(
                    converter: (state, dispatch) => state.mineState,
                    builder: (_context, viewModel) => {
                        var data = index == 0 ? viewModel.futureEventsList : viewModel.pastEventsList;
                        if (index == 0) {
                            if (viewModel.futureListLoading) return new GlobalLoading();
                            if (data.Count <= 0) return new BlankView("暂无我的即将开始活动");
                        }

                        if (index == 1) {
                            if (viewModel.pastListLoading) return new GlobalLoading();
                            if (data.Count <= 0) return new BlankView("暂无我的往期活动");
                        }

                        return ListView.builder(
                            physics: new AlwaysScrollableScrollPhysics(),
                            itemCount: data.Count,
                            itemBuilder: (cxt, idx) => {
                                var model = data[idx];
                                return new EventCard(
                                    model,
                                    () => {
                                        StoreProvider.store.Dispatch(new MainNavigatorPushToEventDetailAction {
                                            EventId = model.id,
                                            EventType = model.mode == "online" ? EventType.onLine : EventType.offline
                                        });
                                    }
                                );
                            }
                        );
                    }
                )
            );
        }
    }
}