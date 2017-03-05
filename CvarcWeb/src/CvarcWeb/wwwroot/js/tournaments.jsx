import {Component} from 'react';
import OlympicTournament from './olympicTournament';
import GroupTournament from './groupTournament';

class Tournaments extends Component {
    constructor(props) {
        super(props);
        this.state = {
            tournaments: []
        };
    }

    componentWillMount() {
        fetch('Tournaments/all', {
                credentials: "same-origin",
                headers: { "Content-Type": "application/json", "enctype": "json" }
        })
            .then(data => data.json())
            .then(data => {
                this.setState({ tournaments: data });
            });
    }

    componentDidMount() {
        $("#team-selector").on("keyup click input", function() {
            if (this.value.length > 0) {
                [$(".game-top"), $(".game-bottom"), $(".group-tournament tbody tr")].forEach(elements => elements.removeClass("match").filter(function() {
                    return $(this).find(".team-name").text().toLowerCase().indexOf($("#team-selector").val().toLowerCase()) !== -1;
                }).addClass("match"));
            }
            else {
                [$(".game"), $(".group-tournament tbody tr")].forEach(elements => elements.removeClass("match"));
            }
        });
    }

    render() {
        const allTournaments = this.state.tournaments.map(t => 
            <li className="tournament-list-item" key={t} onClick={() => this.showTournament(t)}>
                <label><input className="tournament-selector" type="radio" name="tournament-name"/><span className="tournament-name">{t}</span></label>
            </li>);
        const selectedTournament = this.state.currentTournament 
            ? this.state.currentTournament.Type === 0 
                ? (<OlympicTournament tournament={this.state.currentTournament}/>)
                : (<GroupTournament tournament = {this.state.currentTournament}/>)
            : null;
        return (
            <div className="tournaments">
                <ul className="tournaments-list">
                    {allTournaments}
                </ul>
                {selectedTournament}
            </div>
        );
        }

    showTournament(tournamentName) {
        fetch(`Tournaments/${tournamentName}`, {
                credentials: "same-origin",
                headers: { "Content-Type": "application/json", "enctype": "json" }
            })
            .then(data => data.json())
            .then(data => {
                this.setState({ currentTournament: data });
            });
    }
};

export default Tournaments;