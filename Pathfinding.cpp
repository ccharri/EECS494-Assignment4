//
//  Pathfinding.cpp
//  game
//
//  Created by Christopher Harris on 9/26/13.
//
//

#include "Pathfinding.h"

#include <unordered_map>
#include <queue>
#include <algorithm>
#include <zenilib.h>
#include <functional>

#include "Square.h"

using namespace Zeni;
using namespace std;
using namespace std::placeholders;

void reconstruct_path(unordered_map<Square*, Square*>& from_map, Square* destination_, vector<Square*>& return_list);
void evaluate_neighbors(Square* current, Square* destination, vector<Square*>& closed_list, vector<Square*>& open_list, unordered_map<Square*, Square*>& from_map, unordered_map<Square*, float>& g_score_map, unordered_map<Square*, float>& f_score_map);
void evaluate_neighbor(Square* current, Square* neighbor, Square* destination, vector<Square*>& closed_list, vector<Square*>& open_list, unordered_map<Square*, Square*>& from_map, unordered_map<Square*, float>& g_score_map, unordered_map<Square*, float>& f_score_map);
float heuristic_cost_estimate(Square* start, Square* goal);

vector<Square*> AStar(Square* start_, Square* destination_)
{
    vector<Square*> closed_list;
    vector<Square*> open_list;
    unordered_map<Square*, Square*> from_map;
    unordered_map<Square*, float> g_score_map;
    unordered_map<Square*, float> f_score_map;
    
    g_score_map[start_] = 0;
    f_score_map[start_] = g_score_map[start_] + heuristic_cost_estimate(start_, destination_);
    
    open_list.push_back(start_);
    
    while (open_list.size())
    {
        Square* next = open_list.front();

        if(next == destination_)
        {
            vector<Square*> return_list;
            reconstruct_path(from_map, destination_, return_list);
            auto start = find(return_list.begin(), return_list.end(), start_);
            if(start != return_list.end()) return_list.erase(start);
            return return_list;
        }
        
        open_list.erase(open_list.begin());
        closed_list.insert(lower_bound(closed_list.begin(), closed_list.end(), next), next);

        evaluate_neighbors(next, destination_, closed_list, open_list, from_map, g_score_map, f_score_map);
    }
    
    return vector<Square*>();
}

void evaluate_neighbors(Square* current, Square* destination, vector<Square*>& closed_list, vector<Square*>& open_list, unordered_map<Square*, Square*>& from_map, unordered_map<Square*, float>& g_score_map, unordered_map<Square*, float>& f_score_map)
{
    Square* neighbor = current->getNorth();
    evaluate_neighbor(current, neighbor, destination, closed_list, open_list, from_map, g_score_map, f_score_map);
    neighbor = current -> getEast();
    evaluate_neighbor(current, neighbor, destination, closed_list, open_list, from_map, g_score_map, f_score_map);
    neighbor = current -> getSouth();
    evaluate_neighbor(current, neighbor, destination, closed_list, open_list, from_map, g_score_map, f_score_map);
    neighbor = current -> getWest();
    evaluate_neighbor(current, neighbor, destination, closed_list, open_list, from_map, g_score_map, f_score_map);
}

void evaluate_neighbor(Square* current, Square* neighbor, Square* destination, vector<Square*>& closed_list, vector<Square*>& open_list, unordered_map<Square*, Square*>& from_map, unordered_map<Square*, float>& g_score_map, unordered_map<Square*, float>& f_score_map)
{
    if(!neighbor) return;
    if(!neighbor->isPathable()) return;
    
    float tentative_g_score = g_score_map[current] + Vector2f(neighbor->getRealPosition() - current->getRealPosition()).magnitude();
    if(binary_search(closed_list.begin(), closed_list.end(), neighbor) && (tentative_g_score >= g_score_map[neighbor]))
    {
        return;
    }
    
    bool inOpenList = binary_search(open_list.begin(), open_list.end(), neighbor, [&](Square* lhs, Square* rhs) {
        return f_score_map[lhs] < f_score_map[rhs];
    });
    
    if((!inOpenList) || (tentative_g_score < g_score_map[neighbor]))
    {
        from_map[neighbor] = current;
        g_score_map[neighbor] = tentative_g_score;
        f_score_map[neighbor] = g_score_map[neighbor] + heuristic_cost_estimate(neighbor, destination);
        
        if(inOpenList)
        {
            sort(open_list.begin(), open_list.end(), [&](Square* lhs, Square* rhs) {
                return f_score_map[lhs] < f_score_map[rhs];
            });
        }
        else
        {
            open_list.insert(lower_bound(open_list.begin(), open_list.end(), neighbor, [&](Square* lhs, Square* rhs) {
                return f_score_map[lhs] < f_score_map[rhs];
            }), neighbor);
        }
    }
    
}

float heuristic_cost_estimate(Square* start, Square* goal)
{
    return Vector2f(goal->getRealPosition() - start->getRealPosition()).magnitude();
}

void reconstruct_path(unordered_map<Square*, Square*>& from_map, Square* destination_, vector<Square*>& return_list)
{
    if (from_map.find(destination_) != from_map.end())
    {
        Square* from = from_map[destination_];
        return_list.insert(return_list.begin(), destination_);
        reconstruct_path(from_map, from, return_list);
    }
    else
    {
        return_list.push_back(destination_);
    }
}
