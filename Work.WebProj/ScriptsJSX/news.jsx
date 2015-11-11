var GridRow = React.createClass({
	mixins: [React.addons.LinkedStateMixin], 
	getInitialState: function() {  
		return { 
		};  
	},
	alertInfo:function(){
		console.log(this.props.itemData.field1);
	},
	delCheck:function(i,chd){
		this.props.delCheck(i,chd);
	},
	render:function(){
		return (
				<tr>
					<td><GridCheckDel iKey={this.props.ikey} chd={this.props.itemData.check_del} delCheck={this.delCheck} /></td>
					<td><GridButtonModify iKey={this.props.ikey} /></td>
					<td>{this.props.itemData.title}</td>
					<td>{this.props.itemData.category}</td>
					<td>{this.props.itemData.set_date}</td>
					<td>{this.props.itemData.is_open}</td>
				</tr>
			);
		}
});

//表單登錄
var GirdForm = React.createClass({
	mixins: [React.addons.LinkedStateMixin], 
	getInitialState: function() {  
		return {
			gridData:{rows:[],page:1},
			edit_type:0
		};  
	},
	getDefaultProps:function(){
		return{			
		};
	},	
	componentWillMount:function(){
		//在輸出前觸發，只執行一次如果您在這個方法中呼叫 setState() ，會發現雖然 render() 再次被觸發了但它還是只執行一次。
	},
	componentDidMount:function(){
		//只在客戶端執行一次，當渲染完成後立即執行。當生命週期執行到這一步，元件已經俱有 DOM 所以我們可以透過 this.getDOMNode() 來取得 DOM 。
		//如果您想整和其他 Javascript framework ，使用 setTimeout, setInterval, 或者是發動 AJAX 請在這個方法中執行這些動作。
		this.queryGridData(1);
	},
	componentWillReceiveProps:function(nextProps){
		//當元件收到新的 props 時被執行，這個方法在初始化時並不會被執行。使用的時機是在我們使用 setState() 並且呼叫 render() 之前您可以比對 props，舊的值在 this.props，而新值就從 nextProps 來。
	},
	shouldComponentUpdate:function(nextProps,nextState){
		/*
		如同其命名，是用來判斷元件是否該更新，當 props 或者 state 變更時會再重新 render 之前被執行。這個方法在初始化時不會被執行，或者當您使用了 forceUpdate 也不會被執行。
		當你確定改變的 props 或 state 並不需要觸發元件更新時，在這個方法中適當的回傳 false 可以提升一些效能。

		shouldComponentUpdate: function(nextProps, nextState) {
  			return nextProps.id !== this.props.id;
		}

		如果 shouldComponentUpdate 回傳 false 則 render() 就會完全被跳過直到下一次 state 改變，此外 componentWillUpdate 和 componentDidUpdate 將不會被觸發。
		當 state 產生異動，為了防止一些奇妙的 bug 產生，預設 shouldComponentUpdate 永遠回傳 true ，不過如果您總是使用不可變性(immutable)的方式來使用 state，並且只在 render 讀取它們那麼你可以複寫 shouldComponentUpdate
		或者是當效能遇到瓶頸，特別是需要處理大量元件時，使用 shouldComponentUpdate 通常能有效地提升速度。
		*/
		return true;
	},
	componentWillUpdate:function(nextProps,nextState){
		/*
			當收到 props 或者 state 立即執行，這個方法在初始化時不會被執行，使用時機通常是在準備更新之前。
			注意您不能在這個方法中使用 this.setState()。如果您需要在修改 props 之後更新 state 請使用 componentWillReceiveProps 取代
		*/
	},
	componentDidUpdate:function(prevProps, prevState){
		/*
			在元件更新之後執行。這個方法同樣不在初始化時執行，使用時機為當元件被更新之後需要執行一些操作。
		*/
	},
	componentWillUnmount:function(){
		//元件被從 DOM 卸載之前執行，通常我們在這個方法清除一些不再需要地物件或 timer。
	},
	handleChange: function (newValue) {  
		//this.setState({text: newValue});
	}, 
	handleSubmit: function(e) {
		e.preventDefault();
		alert('submit');
		return;
	},
	delCheck:function(i,chd){

		var newState = this.state;
		this.state.gridData.rows[i].check_del = !chd;
		this.setState(newState);
	},
	queryGridData:function(page){
					var jqxhr = $.ajax({
							type: "GET",
							url: gb_approot + 'api/NewsData',
							data: {page:page},
							dataType: 'json'
							})
					.done(function(data, textStatus, jqXHRdata) {
						this.setState({gridData:data});
					 }.bind(this))
					.fail(function( jqXHR, textStatus, errorThrown ) {
						showAjaxError(errorThrown);
					});
	},
	insertType:function(){
		this.setState({edit_type:1});
	},
	updateType:function(){
		this.setState({edit_type:2});
	},
	noneType:function(){
		this.setState({edit_type:0});
	},
	render: function() {
		var oper = null;
		if(this.state.edit_type==0)
		{
			oper =(<span>
				<ul className="breadcrumb">
                    <li><i className="fa-list-alt"></i> {this.props.MenuName}</li>
                </ul>
                <h3 className="title">
			    	<span>{this.props.Caption}</span>
				</h3>
				<form>
				    <div className="table-responsive">
				        <div className="table-header">
				            <div className="table-filter">
				                <div className="form-inline">
				                    <div className="form-group">
				                        <input type="text" className="form-control" />
				                    </div>
				                    <button className="btn-primary" type="submit"><i className="fa-search"></i> 搜尋</button>
				                </div>
				            </div>
				        </div>
				        <table>
				            <thead>
				                <tr>
				                	<th className="col-xs-1">刪除</th>
				                    <th className="col-xs-1">修改</th>
				                    <th className="col-xs-4">標題</th>
				                    <th className="col-xs-2">分類</th>
				                    <th className="col-xs-3">日期</th>
				                    <th className="col-xs-1">狀態</th>
				                </tr>
				            </thead>
				            <tbody>
					            {
					            	this.state.gridData.rows.map(function(itemData,i) {
										return <GridRow 
										key={itemData.news_id} 
										ikey={i} 
										itemData={itemData} 
										delCheck={this.delCheck}
										/>;
									}.bind(this))
					            }
				            </tbody>
				        </table>
				    </div>
				    <GridNavPage 
				    	StartCount={this.state.gridData.startcount}
				    	EndCount={this.state.gridData.endcount}
				    	RecordCount={this.state.gridData.records}
				    	TotalPage={this.state.gridData.total}
				    	NowPage={this.state.gridData.page}
				    	onQueryGridData={this.queryGridData}
				    	InsertType={this.insertType}
				    	UpdateType={this.insertType}
				    />
				</form>
			</span>);
		}else if(this.state.edit_type==1)
		{
			oper=(<span>    
					<form className="form-horizontal" onSubmit={this.handleSubmit}>
				        <div className="modal-body">
				            基本資料
				            <div className="form-group">
				                <label className="col-xs-2 control-label">標題</label>
				                <div className="col-xs-10">
				                    <input type="text" className="form-control" 
				                           maxlength="16"
				                           required />
				                </div>
				            </div>

				            <div className="form-group">
				                <label className="col-xs-2 control-label">分類</label>
				                <div className="col-xs-10">
				                    <select className="form-control" required></select>
				                </div>
				            </div>

				            <div className="form-group">
				                <label className="col-xs-2 control-label">日期</label>
				                <div className="col-xs-10">
				                    <input type="date" className="form-control" required />
				                </div>
				            </div>
				        </div>
				        <div className="modal-footer">
				            <button type="submit" className="btn-primary"><i className="fa-check"></i> 儲存</button>
				            <button type="button" onClick={this.noneType}><i className="fa-times"></i> 回前頁</button>
				        </div>
	    			</form>				
				</span>);
		}else{
			oper=(<span>No Page</span>);
		}

		return oper;
	}
});