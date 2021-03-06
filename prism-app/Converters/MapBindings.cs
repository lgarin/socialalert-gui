﻿using Bing.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Socialalert.Converters
{
    public static class MapBindings
    {
        public static Location GetMapLocation(DependencyObject obj)
        {
            return (Location)obj.GetValue(MapLocationProperty);
        }

        public static void SetMapLocation(DependencyObject obj, Location value)
        {
            obj.SetValue(MapLocationProperty, value);
        }

        public static readonly DependencyProperty MapLocationProperty = DependencyProperty.RegisterAttached("MapLocation", typeof(Location), typeof(MapBindings), new PropertyMetadata(null, OnMapLocationChanged));

        private static void OnMapLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                MapLayer.SetPosition(d, (Location)e.NewValue);
            }
        }

        public static Location GetMapCenter(DependencyObject obj)
        {
            return (Location)obj.GetValue(MapCenterProperty);
        }

        public static void SetMapCenter(DependencyObject obj, Location value)
        {
            obj.SetValue(MapCenterProperty, value);
        }

        public static readonly DependencyProperty MapCenterProperty = DependencyProperty.RegisterAttached("MapCenter", typeof(Location), typeof(MapBindings), new PropertyMetadata(null, OnMapCenterChanged));

        private static void OnMapCenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var map = d as Bing.Maps.Map;
            if (map != null)
            {
                map.Center = (Location)e.NewValue;
            }
        }


        public static double GetZoomLevel(DependencyObject obj)
        {
            return (double)obj.GetValue(ZoomLevelProperty);
        }

        public static void SetZoomLevel(DependencyObject obj, double value)
        {
            obj.SetValue(ZoomLevelProperty, value);
        }

        public static readonly DependencyProperty ZoomLevelProperty = DependencyProperty.RegisterAttached("ZoomLevel", typeof(double), typeof(MapBindings), new PropertyMetadata(null, OnZoomLevelChanged));

        private static void OnZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var map = d as Bing.Maps.Map;
            if (map != null)
            {
                map.ZoomLevel = (double)e.NewValue;
            }
        }


        public static LocationRect GetMapBounds(DependencyObject obj)
        {
            return (LocationRect)obj.GetValue(MapBoundsProperty);
        }

        public static void SetMapBounds(DependencyObject obj, LocationRect value)
        {
            obj.SetValue(MapBoundsProperty, value);
        }

        public static readonly DependencyProperty MapBoundsProperty = DependencyProperty.RegisterAttached("MapBounds", typeof(LocationRect), typeof(MapBindings), new PropertyMetadata(null, OnMapBoundsChanged));

        private static void OnMapBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var map = d as Bing.Maps.Map;
            if (map != null && e.NewValue != null)
            {
                map.SetView((LocationRect)e.NewValue);
            }
        }
    }
}
