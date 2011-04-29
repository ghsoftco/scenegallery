/*
Grid.cpp
Written by Matthew Fisher
*/

#pragma once

#include "Grid.h"

template <class type> void Grid<type>::Allocate(unsigned int Rows, unsigned int Cols)
{
    _Rows = Rows;
    _Cols = Cols;
    if(_Data)
    {
        delete[] _Data;
    }
    _Data = new type[Rows * Cols];
}

template <class type> void Grid<type>::Clear(const type &T)
{
    const unsigned int TotalEntries = _Rows * _Cols;
    for(unsigned int Index = 0; Index < TotalEntries; Index++)
    {
        _Data[Index] = T;
    }
}

template <class type> Grid<type>::Grid()
{
    _Rows = 0;
    _Cols = 0;
    _Data = NULL;
}

template <class type> Grid<type>::Grid(unsigned int Rows, unsigned int Cols)
{
    _Rows = Rows;
    _Cols = Cols;
    _Data = new type[Rows * Cols];
}

template <class type> Grid<type>::Grid(const Grid<type> &G)
{
    _Rows = G._Rows;
    _Cols = G._Cols;

    const unsigned int TotalEntries = _Rows * _Cols;
    _Data = new type[TotalEntries];
    for(unsigned int Index = 0; Index < TotalEntries; Index++)
    {
        _Data[Index] = G._Data[Index];
    }
}

template <class type> Grid<type>::~Grid()
{
    FreeMemory();
}

template <class type> void Grid<type>::FreeMemory()
{
    _Rows = 0;
    _Cols = 0;
    if(_Data != NULL)
    {
        delete[] _Data;
        _Data = NULL;
    }
}

template <class type> Grid<type>& Grid<type>::operator = (const Grid<type> &G)
{
    if(_Data)
    {
        delete[] _Data;
    }
    _Rows = G._Rows;
    _Cols = G._Cols;

    const unsigned int TotalEntries = _Rows * _Cols;
    _Data = new type[TotalEntries];
    for(unsigned int Index = 0; Index < TotalEntries; Index++)
    {
        _Data[Index] = G._Data[Index];
    }

    return *this;
}
