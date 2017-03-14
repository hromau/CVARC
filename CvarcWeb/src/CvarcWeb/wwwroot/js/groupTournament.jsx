import {Component} from 'react';
import {getMainScore, isMainScore, sumOtherScores, getWinner} from './gameHelper';

class GroupTournament extends Component {
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
                            {table[i].map((g, j) => <td key={j}>{g === null ? "X" : `${getMainScore(g.TeamGameResults[j < i ? 0 : 1])}:${getMainScore(g.TeamGameResults[j < i ? 1 : 0])}`}</td>)}
                        </tr>
                    ))}
                </tbody>
            </table>
        );
    }
}

export default GroupTournament;