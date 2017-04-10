import {Component} from 'react';
import {getMainScore, isMainScore, sumOtherScores, getWinner} from './gameHelper';

class GroupTournament extends Component {
    renderTable(table) {
        const name = table.GroupName;
        table = table.Games;
        const teamNames = [table[0][1].TeamGameResults[0].Team.Name].concat(table[0].filter(g => g !== null).map(g => g.TeamGameResults[1].Team.Name));
        return (
            <table className="group-tournament" key={name}>
                <thead>
                    <tr>
                        <th></th>
                        {teamNames.map((n, i) => <th className="team-name" key={i}>{n}</th>)}
                    </tr>
                </thead>
                <tbody>
                    {teamNames.map((n, i) => (
                        <tr key={i}>
                            <th className="team-name">{n}</th>
                            {table[i].map((g, j) => <td key={j}>{g === null ? "X" : <a href={`/Logs/${g.GameId}`}>{getMainScore(g.TeamGameResults[j < i ? 0 : 1])}:{getMainScore(g.TeamGameResults[j < i ? 1 : 0])}</a>}</td>)}
                        </tr>
                    ))}
                </tbody>
            </table>
        );
    }
    render() {
        const tables = this.props.tournament.Groups;
        return (
            <div>
                {tables.map(t => this.renderTable(t))}
            </div>
        );
    }
}

export default GroupTournament;