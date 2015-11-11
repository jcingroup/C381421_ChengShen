var GridButtonModify = React.createClass({
	getInitialState: function() {  
		return { 
		};  
	},
	onClick:function(e){
		alert(this.props.iKey)
	},
	render:function(){
		return (
			<button type="button" onClick={this.onClick}>Alert</button>
			);
	}
});

var GridCheckDel = React.createClass({
	getInitialState: function() {  
		return { 
		};  
	},
	onChange:function(e){
		this.props.delCheck(this.props.iKey,this.props.chd);
	},
	render:function(){
		return (
			<input type="checkbox" checked={this.props.chd} onChange={this.onChange} />
			);
	}
});


var GridNavPage = React.createClass({
	getInitialState: function() {  
		return {
		};  
	},
	firstPage:function(){
		this.props.onQueryGridData(1);
	},
	lastPage:function(){
		this.props.onQueryGridData(this.props.TotalPage);
	},
	nextPage:function(){
		if(this.props.NowPage < this.props.TotalPage){
			this.props.onQueryGridData(this.props.NowPage + 1);
		}
	},
	prvePage:function(){
		if(this.props.NowPage > 1){
			this.props.onQueryGridData(this.props.NowPage - 1);
		}
	},
	render:function(){
		var oper = null;

		oper = (
			<div className="table-footer">
				<div className="pull-left">
					<button className="btn-link text-success"
					type="button"
					onClick={this.props.InsertType}>
						<i className="fa-plus-circle"></i> 新增
					</button>
					<button className="btn-link text-danger" type="button">
						<i className="fa-trash-o"></i> 刪除
					</button>
				</div>
				<small className="pull-right">第{this.props.StartCount}-{this.props.EndCount}筆，共{this.props.RecordCount}筆</small>
			
				<ul className="pager">
					<li>
						<button type="button" title="移至第一頁" tabIndex="-1" onClick={this.firstPage}>
						<i className="fa-angle-double-left"></i>
						</button>
					</li>
					<li>
						<button type="button" title="下一頁" tabIndex="-1" onClick={this.prvePage}>
						<i className="fa-angle-left"></i>
						</button>
					</li>
					<li className="form-inline">
						<div className="form-group">
							<label>第</label>
							<input className="form-control" type="number" min="1" tabIndex="-1" value={this.props.NowPage} />
							<label>頁，共{this.props.TotalPage}頁</label>
						</div>
					</li>
					<li>
						<button type="button" title="@Resources.Res.NextPage" tabIndex="-1" onClick={this.nextPage}>
							<i className="fa-angle-right"></i>
						</button>
					</li>
					<li>
						<button type="button" title="移至最後一頁" tabIndex="-1" onClick={this.lastPage}>
							<i className="fa-angle-double-right"></i>
						</button>
					</li>
				</ul>
			</div>
			);

	return oper;
}
});