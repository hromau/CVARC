import {Component} from 'react';

class TeamNameFilter extends Component {
    componentDidMount() {
        this.setSpinner();
        let changed = false;
        $(this.__input).on('input', () => {
            changed = true;
            new Promise((resolve, reject) => {
                setTimeout(() => {
                    if (!changed) {
                        reject();
                    }
                    changed = false;
                    resolve();
                }, 750);
            }).then(() => {
                this.showSpinner();
                this.props.pageState.filters.Page = 0;
                this.props.loadGames().then(() => this.hideSpinner());
            })
            .catch(() => {});
        });
    }

    setSpinner() {
        this.__spinnerContainer.appendChild(document.querySelector("#spinner-source"));
        this.__spinner = this.__spinnerContainer.firstElementChild;
        this.__spinner.removeAttribute("id");
        this.hideSpinner();
    }

    hideSpinner() {
        this.__spinner.style.display = "none";
    }

    showSpinner() {
        this.__spinner.style.display = "block";
    }

    getValue() {
        return this.__input.value;
    }

    setValue(value) {
        this.__input.value = value;
    }

    render() {
        return (
            <div className="team-name-filter">
                <div className="row-fluid">
                    <div className="form-group">
                        <input type="text" className="typeahead form-control" placeholder="Search for team (start typing)" autoComplete="off" ref={(c) => this.__input = c}/>
                    </div>
                </div>
                <div ref={(c) => this.__spinnerContainer = c}/>
            </div>);
    }
};

export default TeamNameFilter;