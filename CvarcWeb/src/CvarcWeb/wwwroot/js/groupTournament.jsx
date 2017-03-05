import {Component} from 'react';

class GroupTournament extends Component {
    getMainScores(tgr) {
        return tgr.Results.filter(r => r.ScoresType === "MainScores")[0].Scores;
    }

    getWinner(match) {
        if (this.getMainScores(match.TeamGameResults[0]) > this.getMainScores(match.TeamGameResults[1])) {
            return match.TeamGameResults[0].Team.Name;
        }
        return match.TeamGameResults[1].Team.Name;
    }

    render() {
        const table = this.props.tournament.Games;
        const teamNames = [table[0][1].TeamGameResults[1].Team.Name].concat(table[0].filter(g => g !== null).map(g => g.TeamGameResults[0].Team.Name));
        return (
            <table className="group-tournament">
                <thead>
                    <tr>
                        <th></th>
                        {teamNames.map(n => <th className="team-name" key={n}>{n}</th>)}
                    </tr>
                </thead>
                <tbody>
                    {teamNames.map((n, i) => (
                        <tr key={i}>
                            <th className="team-name">{n}</th>
                            {table[i].map((g, j) => <td key={j}>{g === null ? "X" : `${this.getMainScores(g.TeamGameResults[j < i ? 0 : 1])}:${this.getMainScores(g.TeamGameResults[j < i ? 1 : 0])}`}</td>)}
                        </tr>
                    ))}
                </tbody>
            </table>
        );
    }
}

export default GroupTournament;