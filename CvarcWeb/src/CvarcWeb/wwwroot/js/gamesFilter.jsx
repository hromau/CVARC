import {Component} from 'react';
import Games from './games';
import TeamNameFilter from './teamNameFilter';
import Paging from './paging';

class GamesFilter extends Component {
    constructor() {
        super();
        this.state = {
            games: [],
            total: 0,
            filters: document.location.search.substring(1) 
                            ? this.parseQuery(document.location.search) 
                            : { TeamName: "", Page: 0 }
        };
        
        window.onpopstate = (e) => {
            if (!e.state) {
                return;
            }
            this.setState(e.state);
            this.setFiltersValues(this.state.filters);
        }
    }

    componentDidMount() {
        this.setFiltersValues();
        this.loadGames();
    }

    buildQuery(params) {
        return (Object.keys(params)
                      .filter(k => !!params[k])
                      .map(k => {
                            const v = params[k];
                            return `${encodeURIComponent(k)}=${encodeURIComponent(v)}`;
                        })
                      .join("&")) || "";
    }

    setFiltersValues() {
        this.__teamNameFilter.setValue(decodeURIComponent(this.state.filters.TeamName || ""));
    }

    parseQuery(query) {
        return query.substr(0, 1) === "?" && query.length > 1
            ? query.substr(1)
                   .split("&")
                   .reduce((res, pair) => {
                       var [k, v] = pair.split("=");
                       res[k] = v;
                       return res;
                   }, {})
            : {};
    }

    loadGames() {
        this.state.filters.TeamName = this.__teamNameFilter.getValue();
        let query = this.buildQuery(this.state.filters);
        return fetch(`Games/Find?${query}`, {
                    credentials: "same-origin",
                    headers: { "Content-Type": "application/json", "enctype": "json" }
                }
            )
            .then(data => data.json())
            .then(data => {
                this.setState({ games: data.games, total: data.total, filters: this.state.filters});
                history.pushState(this.state, "", query ? `?${query}` : document.location.pathname);
            });
    }

    changePage(page) {
        this.state.filters.Page = page;
        new Promise(resolve=> {
            this.__teamNameFilter.showSpinner();
            resolve();
        }).then(() => this.loadGames().then(() => this.__teamNameFilter.hideSpinner()));
    }

    render() {
        return (
            <div>
                <div className="filters">
                    <TeamNameFilter loadGames={(query) => this.loadGames(query)} pageState={this.state} ref={(c) => this.__teamNameFilter = c}/>
                    <Paging total={this.state.total} currentPage={this.state.filters.Page} changePage={(n) => this.changePage(n)}/>
                </div>
                <Games games={this.state.games}/>
            </div>
        );
    }
};

export default GamesFilter;