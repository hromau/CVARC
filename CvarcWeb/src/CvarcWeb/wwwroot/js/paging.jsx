import {Component} from 'react';

class Paging extends Component {
    render() {
        const pageSize = 30;
        const total = this.props.total;
        const current = this.props.currentPage || 0;
        const pageNumbers = [];
        const onClick = this.props.changePage;
        for (let i = 5; i >= -5; i--) {
            if (current - i >= 0 && current - i < total / pageSize) {
                pageNumbers.push(current - i);
            }
        }
        return (
            <div className="paging">
                {
                    pageNumbers.map(n => {
                        var isCurrent = n === parseInt(current.toString(), 10);
                        var className = isCurrent ? "current page-number" : "page-number";
                        return (<span className={className} onClick={() => !isCurrent && onClick(n) } key={n}>{n+1}</span>);
                    })
                }
            </div>
        );
    }
};

export default Paging;