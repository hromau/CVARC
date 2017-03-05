import {Component} from 'react';
import Game from './game';

class Games extends Component {
    render() {
        const gameItems = this.props.games.map(g => <Game {...g} key={g.GameId}/>);
        return (
            this.props.games.length > 0
                ? <div className="games-list">{gameItems}</div>
                : <div className="empty-games-list"><span>Not found :(</span></div>
        );
    }
};

export default Games;