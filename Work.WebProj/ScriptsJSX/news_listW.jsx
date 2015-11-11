//[1]
//主元件 Tabs集合區段
var PageContent = React.createClass({
	mixins: [React.addons.LinkedStateMixin], 
	getInitialState: function() {  
		return { 
			gridData:{rows:[]},
			page:1,
			y:null
		};  
	},		
	componentDidMount:function(){
		this.queryData(this.state.page);
		return;
	},
	yearChange:function(event){
		this.queryDataY(event.target.value);
	},
	queryData:function(page){
		jqGet(this.props.dataUrl,{page:page,y:this.state.y})
		.done(function(data, textStatus, jqXHRdata) {
			this.setState({gridData:data,page:page});
		}.bind(this))
		.fail(function( jqXHR, textStatus, errorThrown ) {
			showAjaxError(errorThrown);
		});
	},
	queryDataY:function(y){
		jqGet(this.props.dataUrl,{page:1,y:y})
		.done(function(data, textStatus, jqXHRdata) {
			this.setState({gridData:data,page:1,y:y});
		}.bind(this))
		.fail(function( jqXHR, textStatus, errorThrown ) {
			showAjaxError(errorThrown);
		});
	},
	pageClick:function(page){
		this.queryData(page);
		return false;
	},
	pagePrve:function(){
		if(this.state.page > 1){
			this.queryData(this.state.page - 1);
		}
		return false;
	},
	pageNext:function(){
		if(this.state.page < this.state.gridData.total){
			this.queryData(this.state.page + 1);
		}
		return false;
	},
	render:function(){

		var dpage = [];
		for(var i=1;i<=this.state.gridData.total;i++){
			if(this.state.gridData.page==i){
				dpage.push(<a key={i} className="current">{i}</a>);
			}else{
				dpage.push(<a key={i} href="#" onClick={this.pageClick.bind(this,i)}>{i}</a>);
			}
		}

		var outHtml = (
		<div>
			<div className="search">
				<span>選擇查詢年度</span>
				<select onChange={this.yearChange}>
					<option value="">選擇年度</option>
					<option value="2015">2015</option>
					<option value="2014">2014</option>
					<option value="2013">2013</option>
					<option value="2012">2012</option>
				</select>
				<span>熱門查詢:</span>
				<nav>
					<a href="">姊妹會</a>
					<a href="">理事會</a>
					<a href="">慈善活動</a>
					<a href="">奧瑞岡</a>
				</nav>
			</div>
			<table className="news-list">
				<tr>
					<th>活動時間</th>
					<th>標題</th>
				</tr>
				{
					this.state.gridData.rows.map(function(itemData,i) {
						var subOutHtml = 
						<tr key={i}>
							<td>{moment(itemData.活動日期).format('YYYY-MM-DD')}</td>
							<td><a href={this.props.contextUrl + itemData.流水號}>{itemData.標題}</a></td>
						</tr>;
						return subOutHtml;
					}.bind(this))
				}
			</table>
			<nav className="pager">
				<a href="#" onClick={this.pagePrve}>上一頁</a>
				{dpage}				
				<a href="#" onClick={this.pageNext}>下一頁</a>
			</nav>
		</div>
			);

		return outHtml;
	}
});

//元件嵌入 div id:PageContent
var comp = React.render(<PageContent dataUrl={gb_approot + 'api/GetAction/GetNewsWWW'} contextUrl={gb_approot + 'News/News_content?id='} />,document.getElementById('PageContent'));