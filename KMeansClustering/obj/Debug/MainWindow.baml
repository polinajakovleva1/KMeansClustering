   M S B A M L   `   `   `  ����K  GKMeansClustering, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:KMeansClusteringKMeansClustering  5   	   !    KMeansClustering.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:KMeansClustering     5   	     � Title$  KMeansClustering��5   	   $	��400��6"   $	��300��6/   .��5	      �  � RowDefinitions 5
   
   �� 5      $	��100 �6   �� 5      $	��100 �6   �� 5      $	��100 �6   �� 5      $��80 �6   .+�5   
   �� $��0��6   $��Bottom=�6   $	��250��6:   $��50��6F   $��Выбрать файл��5      
�������$��18(�6;   {�5      �� *IsDownloadFile+��66   �� *DownloadFile+5      �� 5   
   $��1��6   $��Center=�6   $	��250��6:   $��50��6F   $/��)Кластеризовать данные��5      
�������$��18(�6D   {�5      �� *IsClustering+��64   �� *
Clustering+5      �� 5   
   $��2��6   $	��Top=�6   $	��250��67   $��50��6C   $+��%Сохранить результат��5      
�������$��18(�6B   {�5      �� *IsUploadFile+��64   �� *
UploadFile+5      �� 5   
   $��3��6   $	��Top=�6   $	��250��67   $��50��6C   $��
Выйти��5      
�������$��18(�64   {�5      �� *IsExit+��6.   �� *Exit+5      5       5!      